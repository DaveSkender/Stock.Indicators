require 'mkmf'
require 'rbconfig'

def generate_makefile
  create_makefile("wdm_ext")
end

def generate_dummy_makefile
  File.open("Makefile", "w") do |f|
    f.puts dummy_makefile('wdm_ext').join
  end
end

def windows?
  RbConfig::CONFIG['host_os'] =~ /mswin|mingw|cygwin/
end

if windows? and
  have_library("kernel32") and
  have_header("windows.h")
then
  generate_makefile()
else
  generate_dummy_makefile()
end
