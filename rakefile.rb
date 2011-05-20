COMPILE_TARGET = ENV['config'].nil? ? "release" : ENV['config']
require 'rubygems'
gem 'albacore', '= 0.2.5'
require 'albacore'

include FileTest
#require 'albacore'
load "VERSION.txt"

RESULTS_DIR = "results"
PRODUCT = "FubuMVC"
COPYRIGHT = 'Copyright 2009-2011 Jeremy D. Miller, et al. All rights reserved.';
COMMON_ASSEMBLY_INFO = 'src/CommonAssemblyInfo.cs';
CLR_TOOLS_VERSION = "v4.0.30319"

tc_build_number = ENV["BUILD_NUMBER"]
build_revision = tc_build_number || Time.new.strftime('5%H%M')
build_number = "#{BUILD_VERSION}.#{build_revision}"

props = { :stage => File.expand_path("build"), :stage35 => File.expand_path("build35"), :artifacts => File.expand_path("artifacts") }

desc "**Default**, compiles and runs tests"
task :default => [:compile, :unit_test, :stage, "fx35:compile", "fx35:unit_test", "fx35:stage"]
task :ci => [:default, "package:zips", "package:nuget"]

desc "Update the version information for the build"
assemblyinfo :version do |asm|
  asm_version = BUILD_VERSION + ".0"
  
  begin
    commit = `git log -1 --pretty=format:%H`
  rescue
    commit = "git unavailable"
  end
  puts "##teamcity[buildNumber '#{build_number}']" unless tc_build_number.nil?
  puts "Version: #{build_number}" if tc_build_number.nil?
  asm.trademark = commit
  asm.product_name = PRODUCT
  asm.description = build_number
  asm.version = asm_version
  asm.file_version = build_number
  asm.custom_attributes :AssemblyInformationalVersion => asm_version
  asm.copyright = COPYRIGHT
  asm.output_file = COMMON_ASSEMBLY_INFO
end

desc "Prepares the working directory for a new build"
task :clean do
	#TODO: do any other tasks required to clean/prepare the working directory
	FileUtils.rm_rf props[:stage]
	FileUtils.rm_rf props[:stage35]
    # work around nasty latency issue where folder still exists for a short while after it is removed
    waitfor { !exists?(props[:stage]) }
	Dir.mkdir props[:stage]
    waitfor { !exists?(props[:stage35]) }
	Dir.mkdir props[:stage35]
    
	Dir.mkdir props[:artifacts] unless exists?(props[:artifacts])
end

def waitfor(&block)
  checks = 0
  until block.call || checks >10 
    sleep 0.5
    checks += 1
  end
  raise 'waitfor timeout expired' if checks > 10
end

desc "Compiles the app"
msbuild :compile => [:clean, :version] do |msb|
	msb.command = File.join(ENV['windir'], 'Microsoft.NET', 'Framework', CLR_TOOLS_VERSION, 'MSBuild.exe')
	msb.properties :configuration => COMPILE_TARGET
	msb.solution = "src/HtmlTags.sln"
    msb.targets :Rebuild
    msb.log_level = :verbose
end

desc "Run unit tests"
nunit :unit_test do |nunit|
  nunit.command = 'lib/nunit/nunit-console.exe'
  nunit.assemblies = ["src/HtmlTags.Testing/bin/#{COMPILE_TARGET}/HtmlTags.Testing.dll"]
end

task :stage do
  Dir.glob "src/HtmlTags/bin/#{COMPILE_TARGET}/HtmlTags.*" do |f|
    cp f, props[:stage]
  end
end

namespace :fx35 do
  desc "Compile the app against the .NET Framework 3.5"
  msbuild :compile do |msb|
    output = "bin\\\\#{COMPILE_TARGET}35\\\\"
	msb.command = File.join(ENV['windir'], 'Microsoft.NET', 'Framework', CLR_TOOLS_VERSION, 'MSBuild.exe')
	msb.properties "DefineConstants" => "LEGACY;TRACE", "TargetFrameworkVersion" => "v3.5", :configuration => COMPILE_TARGET, "OutDir" => output , "DocumentationFile" => ""
	msb.solution = "src/HtmlTags.sln"
    msb.targets :Rebuild
    msb.log_level = :verbose
  end

  desc "Run unit tests against the .NET Framework 3.5"
  nunit :unit_test do |nunit|
    nunit.command = 'lib/nunit/nunit-console.exe'
    nunit.assemblies = ["src/HtmlTags.Testing/bin/#{COMPILE_TARGET}/HtmlTags.Testing.dll"]
  end

  task :stage do
    Dir.glob "src/HtmlTags/bin/#{COMPILE_TARGET}35/HtmlTags.*" do |f|
      cp f, props[:stage35]
    end
  end
end

namespace :package do
  desc "Build nuget package"
  task :nuget => [:build_nuget_symbols, :build_nuget_lib] do
    rm_f Dir.glob("HtmlTags*.nuspec")
  end

  nugetpack :build_nuget_lib => :libspec do |nuget|
    nuget.command = "lib/nuget.exe"
    nuget.nuspec = "HtmlTags.nuspec"
    nuget.output = props[:artifacts]
  end
  
  task :build_nuget_symbols => :nuget_symbols do
    cp "#{props[:artifacts]}/HtmlTags.#{build_number}.nupkg", "#{props[:artifacts]}/HtmlTags.#{build_number}.symbols.nupkg"
  end

  nugetpack :nuget_symbols => :symbolspec do |nuget|
    nuget.command = "lib/nuget.exe"
    nuget.nuspec = "HtmlTags-symbols.nuspec"
    nuget.output = props[:artifacts]
  end

  desc "Zip up build artifacts"
  task :zips => [:zip40, :zip35]

  zip :zip40 do |zip|
    zip.directories_to_zip = [props[:stage]]
    zip.output_file = 'htmltags_net40.zip'
    zip.output_path = [props[:artifacts]]
  end

  zip :zip35 do |zip|
    zip.directories_to_zip = [props[:stage35]]
    zip.output_file = 'htmltags_net35.zip'
    zip.output_path = [props[:artifacts]]
  end

  def define_spec(spec) 
    spec.id = "HtmlTags"
    spec.authors = "Jeremy D. Miller, Joshua Flanagan"
    spec.description = "Simple objects for generating HTML"

    spec.owners = spec.authors
    spec.licenseUrl = "https://github.com/DarthFubuMVC/htmltags/raw/master/license.txt"
    spec.projectUrl = "https://github.com/DarthFubuMVC/htmltags"
    spec.iconUrl = "https://github.com/DarthFubuMVC/htmltags/raw/master/logo/HtmlTags_Icon_128x128.png"
    spec.requireLicenseAcceptance = "false"
    spec.tags = "html mvc"

    spec.file 'build\HtmlTags.dll', 'lib\4.0'
    spec.file 'build\HtmlTags.xml', 'lib\4.0'
    spec.file 'build35\HtmlTags.dll', 'lib\2.0'
  end

  nuspec :libspec do |spec|
    define_spec spec
    spec.output_file = "HtmlTags.nuspec"
    spec.version = build_number
  end
  nuspec :symbolspec do |spec|
    define_spec spec
    spec.output_file = "HtmlTags-symbols.nuspec"
    spec.version = build_number
    spec.file 'build\HtmlTags.pdb', 'lib\4.0'
    spec.file 'build35\HtmlTags.pdb', 'lib\2.0'
    spec.file 'src\HtmlTags\**\*.cs', 'src'
  end
end
