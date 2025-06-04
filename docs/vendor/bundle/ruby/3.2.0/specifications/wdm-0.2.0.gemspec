# -*- encoding: utf-8 -*-
# stub: wdm 0.2.0 ruby lib
# stub: ext/wdm/extconf.rb

Gem::Specification.new do |s|
  s.name = "wdm".freeze
  s.version = "0.2.0"

  s.required_rubygems_version = Gem::Requirement.new(">= 0".freeze) if s.respond_to? :required_rubygems_version=
  s.require_paths = ["lib".freeze]
  s.authors = ["Maher Sallam".freeze, "Lars Kanis".freeze]
  s.cert_chain = ["-----BEGIN CERTIFICATE-----\nMIIEBDCCAmygAwIBAgIBAzANBgkqhkiG9w0BAQsFADAoMSYwJAYDVQQDDB1sYXJz\nL0RDPWdyZWl6LXJlaW5zZG9yZi9EQz1kZTAeFw0yNDAyMjgxOTMxNDdaFw0yNTAy\nMjcxOTMxNDdaMCgxJjAkBgNVBAMMHWxhcnMvREM9Z3JlaXotcmVpbnNkb3JmL0RD\nPWRlMIIBojANBgkqhkiG9w0BAQEFAAOCAY8AMIIBigKCAYEAwum6Y1KznfpzXOT/\nmZgJTBbxZuuZF49Fq3K0WA67YBzNlDv95qzSp7V/7Ek3NCcnT7G+2kSuhNo1FhdN\neSDO/moYebZNAcu3iqLsuzuULXPLuoU0GsMnVMqV9DZPh7cQHE5EBZ7hlzDBK7k/\n8nBMvR0mHo77kIkapHc26UzVq/G0nKLfDsIHXVylto3PjzOumjG6GhmFN4r3cP6e\nSDfl1FSeRYVpt4kmQULz/zdSaOH3AjAq7PM2Z91iGwQvoUXMANH2v89OWjQO/NHe\nJMNDFsmHK/6Ji4Kk48Z3TyscHQnipAID5GhS1oD21/WePdj7GhmbF5gBzkV5uepd\neJQPgWGwrQW/Z2oPjRuJrRofzWfrMWqbOahj9uth6WSxhNexUtbjk6P8emmXOJi5\nchQPnWX+N3Gj+jjYxqTFdwT7Mj3pv1VHa+aNUbqSPpvJeDyxRIuo9hvzDaBHb/Cg\n9qRVcm8a96n4t7y2lrX1oookY6bkBaxWOMtWlqIprq8JZXM9AgMBAAGjOTA3MAkG\nA1UdEwQCMAAwCwYDVR0PBAQDAgSwMB0GA1UdDgQWBBQ4h1tIyvdUWtMI739xMzTR\n7EfMFzANBgkqhkiG9w0BAQsFAAOCAYEArBmHSfnUyNWf3R1Fx0mMHloWGdcKn2D2\nBsqTApXU2nADiyppIqRq4b9e7hw342uzadSLkoQcEFOxThLRhAcijoWfQVBcsbV/\nZsCY1qlUTIJuSWxaSyS4efUX+N4eMNyPM9oW/sphlWFo0DgI34Y9WB6HDzH+O71y\nR7PARke3f4kYnRJf5yRQLPDrH9UYt9KlBQm6l7XMtr5EMnQt0EfcmZEi9H4t/vS2\nhaxvpFMdAKo4H46GBYNO96r6b74t++vgQSBTg/AFVwvRZwNSrPPcBfb4xxeEAhRR\nx+LU7feIH7lZ//3buiyD03gLAEtHXai0Y+/VfuWIpwYJAl2BO/tU7FS/dtbJq9oc\ndI36Yyzy+BrCM0WT4oCsagePNb97FaNhl4F6sM5JEPT0ZPxRx0i3G4TNNIYziVos\n5wFER6XhvvLDFAMh/jMg+s7Wd5SbSHgHNSUaUGVtdWkVPOer6oF0aLdZUR3CETkn\n5nWXZma/BUd3YgYA/Xumc6QQqIS4p7mr\n-----END CERTIFICATE-----\n".freeze]
  s.date = "2024-08-04"
  s.description = "Windows Directory Monitor (WDM) is a library which can be used to monitor directories for changes. It's mostly implemented in C and uses the Win32 API for a better performance.".freeze
  s.email = ["maher@sallam.me".freeze, "lars@greiz-reinsdorf.de".freeze]
  s.extensions = ["ext/wdm/extconf.rb".freeze]
  s.files = ["ext/wdm/extconf.rb".freeze]
  s.homepage = "https://github.com/Maher4Ever/wdm".freeze
  s.licenses = ["MIT".freeze]
  s.required_ruby_version = Gem::Requirement.new(">= 2.5".freeze)
  s.rubygems_version = "3.4.20".freeze
  s.summary = "Windows Directory Monitor (WDM) is a threaded directories monitor for Windows.".freeze

  s.installed_by_version = "3.4.20" if s.respond_to? :installed_by_version
end
