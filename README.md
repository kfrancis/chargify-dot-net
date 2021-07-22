# Chargify.NET [![Build and NuGet Publish](https://github.com/kfrancis/chargify-dot-net/actions/workflows/nuget_publish.yml/badge.svg?branch=master)](https://github.com/kfrancis/chargify-dot-net/actions/workflows/nuget_publish.yml)

_A comprehensive C# API wrapper library for accessing [chargify.com](http://www.chargify.com), using XML or JSON to read/write._

* Chargify.NET: [![NuGet version](https://img.shields.io/nuget/v/chargify.svg)](http://www.nuget.org/packages/chargify)
* [![Dependency Status](https://www.versioneye.com/user/projects/5797913674848d0044b8087c/badge.svg?style=flat-square)](https://www.versioneye.com/user/projects/5797913674848d0044b8087c)

## Important SSLv3/POODLE
To correct the error "Could not create SSL/TLS secure channel", update to at least version [1.1.5400.37999](https://www.nuget.org/packages/chargify/) either through the downloads or (recommended) through nuget. I *HIGHLY* recommend using nuget as I release changes through there very often. For more information, see the Chargify blog post ["Dropping support for SSLv3 - may cause API connection problems"](https://chargify.com/blog/dropping-sslv3/). A list of the likely breaking changes that might affect you are listed here: [Latest Breaking Changes](http://chargify.codeplex.com/wikipage?title=Latest%20Breaking%20Changes&referringTitle=Home). Also, if you're still having trouble - make sure you're specifically setting `ChargifyConnect.ProtocolType` to `SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12` (if possible, to handle fall back to lower TLS). If using .NET 4 or under, just set it to `SecurityProtocolType.Tls`.

### Description
A sample ASP.NET website is available at [chargify.clinicalsupportsystems.com](http://chargify.clinicalsupportsystems.com) which demonstrates the following features:
* Subscription (Paid and Freemium) via API or hosted Chargify pages, including coupons.
* Support for "One-time" charges, credits and refunds
* Account migration or change between products
* Metered and quantity based component charges
* Account information management
* Hosted page URL generation (pretty and regular)
* ISO 3166-1 Alpha 2 data built into the library for easy use with country select controls
* Transaction lists and filtering
* Statements (including PDF linking)
* Billing Portal Management API
*Windows Azure* - I've recently added support for Windows Azure apps, just inherit from ChargifyPage and enter the important keys into your role configuration. Neat!
*NOTE*: If there are any issues with the sample website, please feel free to post them on the Issue Tracker on this project site, and I'll make sure they get fixed.
Also, a sample ASP.NET MVC sample is currently being worked on, and will be available at some point. Also, if there is any interest in a Silverlight/Windows Phone solution - let me know.

### Get Started!
New to _Chargify.NET_? 
* Download the [latest release](http://chargify.codeplex.com/releases) or from the NuGet "Package Manager Console", run {{Install-Package chargify}}
* Look at the [Getting Started] page
* Review the [documentation](http://chargify.codeplex.com/documentation)
* Review the [Breaking Changes]
* Join in the [discussion](http://chargify.codeplex.com/discussions)
* Need a quick question answered? Find me on twitter: [@djbyter](http://www.twitter.com/djbyter)

New to _Chargify_?
* Check out their [documentation](http://docs.chargify.com) site
* Submit a "ticket" to their [support area](http://support.chargify.com)

Looking for a DNS managing service? I use [DNSimple](https://dnsimple.com/r/811f4af066782e), and it's super easy. (They are Chargify users as well!)
