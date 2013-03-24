COMPILE_TARGET = ENV['config'].nil? ? "release" : ENV['config']

include FileTest
require 'albacore'
load "VERSION.txt"

RESULTS_DIR = "results"
PRODUCT = "FubuMVC"
COPYRIGHT = 'Copyright 2009-2011 Jeremy D. Miller, et al. All rights reserved.';
COMMON_ASSEMBLY_INFO = 'src/CommonAssemblyInfo.cs';
CLR_TOOLS_VERSION = "v4.0.30319"

buildsupportfiles = Dir["#{File.dirname(__FILE__)}/buildsupport/*.rb"]

if( ! buildsupportfiles.any? )
  # no buildsupport, let's go get it for them.
  sh 'git submodule update --init' unless buildsupportfiles.any?
  buildsupportfiles = Dir["#{File.dirname(__FILE__)}/buildsupport/*.rb"]
end

# nope, we still don't have buildsupport. Something went wrong.
raise "Run `git submodule update --init` to populate your buildsupport folder." unless buildsupportfiles.any?

buildsupportfiles.each { |ext| load ext }


tc_build_number = ENV["BUILD_NUMBER"]
build_revision = tc_build_number || Time.new.strftime('5%H%M')
build_number = "#{BUILD_VERSION}.#{build_revision}"
BUILD_NUMBER = build_number 


props = { :stage => File.expand_path("build"), :stage35 => File.expand_path("build35"), :artifacts => File.expand_path("artifacts") }

desc "**Default**, compiles and runs tests"
task :default => [:compile, :unit_test, :stage]
task :ci => [:update_all_dependencies, :default, :history, :package]

desc "Update the version information for the build"
assemblyinfo :version do |asm|
  asm_version = build_number
  
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
task :clean => [:update_buildsupport] do
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
task :compile => [:restore_if_missing, :clean, :version] do
  MSBuildRunner.compile :compilemode => COMPILE_TARGET, :solutionfile => 'src/HtmlTags.sln', :clrversion => CLR_TOOLS_VERSION

  
  Dir.glob "src/HtmlTags/bin/#{COMPILE_TARGET}/HtmlTags.*" do |f|
    cp f, props[:stage]
  end
end



def copyOutputFiles(fromDir, filePattern, outDir)
  Dir.glob(File.join(fromDir, filePattern)){|file| 		
	copy(file, outDir) if File.file?(file)
  } 
end


desc "Runs unit tests"
task :unit_test => :compile do
  runner = NUnitRunner.new :compilemode => COMPILE_TARGET, :source => 'src', :platform => 'x86'
  runner.executeTests ['HtmlTags.Testing']
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
      cp f, props[:stage]
    end
  end
end

namespace :package do
  desc "Build nuget package"
  task :nuget do
    sh "lib/nuget.exe pack packaging/nuget/htmltags.nuspec -o #{props[:artifacts]} -Version #{build_number}"
    mv "#{props[:artifacts]}/HtmlTags.#{build_number}.nupkg", "#{props[:artifacts]}/HtmlTags.#{build_number}.symbols.nupkg"
    sh "lib/nuget.exe pack packaging/nuget/htmltags.nuspec -o #{props[:artifacts]} -Version #{build_number} -Exclude **\\*.pdb -Exclude **\*.cs"
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
end
