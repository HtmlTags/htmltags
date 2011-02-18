COMPILE_TARGET = ENV['config'].nil? ? "debug" : ENV['config']

include FileTest
require 'albacore'

RESULTS_DIR = "results"
BUILD_NUMBER_BASE = "0.8.0"
PRODUCT = "FubuMVC"
COPYRIGHT = 'Copyright 2009-2011 Jeremy D. Miller, et al. All rights reserved.';
COMMON_ASSEMBLY_INFO = 'src/CommonAssemblyInfo.cs';
CLR_TOOLS_VERSION = "v4.0.30319"

props = { :stage => File.expand_path("build"), :stage35 => File.expand_path("build35"), :artifacts => File.expand_path("artifacts") }

desc "**Default**, compiles and runs tests"
task :default => [:compile, :unit_test]

desc "Update the version information for the build"
assemblyinfo :version do |asm|
  asm_version = BUILD_NUMBER_BASE + ".0"
  
  begin
    commit = `git log -1 --pretty=format:%H`
  rescue
    commit = "git unavailable"
  end
  build_number = "#{BUILD_NUMBER_BASE}.#{Date.today.strftime('%y%j')}"
  tc_build_number = ENV["BUILD_NUMBER"]
  puts "##teamcity[buildNumber '#{build_number}-#{tc_build_number}']" unless tc_build_number.nil?
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
	msb.path_to_command = File.join(ENV['windir'], 'Microsoft.NET', 'Framework', CLR_TOOLS_VERSION, 'MSBuild.exe')
	msb.properties :configuration => COMPILE_TARGET
	msb.solution = "src/HtmlTags.sln"
    msb.targets :Rebuild
    msb.log_level = :verbose
end

desc "Run unit tests"
nunit :unit_test do |nunit|
  nunit.path_to_command = 'lib/nunit/nunit-console.exe'
  nunit.assemblies = "src/HtmlTags.Testing/bin/#{COMPILE_TARGET}/HtmlTags.Testing.dll"
end

namespace :fx35 do
  desc "Compile the app against the .NET Framework 3.5"
  msbuild :compile do |msb|
    output = "bin\\\\#{COMPILE_TARGET}35\\\\"
	msb.path_to_command = File.join(ENV['windir'], 'Microsoft.NET', 'Framework', CLR_TOOLS_VERSION, 'MSBuild.exe')
	msb.properties "DefineConstants" => "LEGACY;TRACE", "TargetFrameworkVersion" => "v3.5", :configuration => COMPILE_TARGET, "OutDir" => output 
	msb.solution = "src/HtmlTags.sln"
    msb.targets :Rebuild
    msb.log_level = :verbose
  end
end
