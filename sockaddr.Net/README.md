![StirlingLabs.sockaddr.Net](https://raw.githubusercontent.com/StirlingLabs/sockaddr.Net/main/sockaddr-dotnet.jpg)

> [libsa](https://github.com/StirlingLabs/libsa) provides cross-platform socket address bindings in C11, designed to be very portable and easy to use.  This allows higher-level network code to use a single format but still communicate effectively with low-level native platform code.

StirlingLabs.sockaddr.Net provides all of [libsa](https://github.com/StirlingLabs/libsa)'s functionality within .Net, avoiding additional memory allocations wherever possible.  It provides an opaque sockaddr* type that is purely pointers or off-gc-heap refs, such that it can be simply cast to/from any other opaque sockaddr* type.

So addresses from any application or API that uses native bindings can be simply cast to StirlingLabs.sockaddr* and then can be manipulated and read without concern for the underlying platform format, then can simply be cast back when being passed back to the application API.

### Why reinvent the wheel?

`sockaddr` isn't going to change once it exists on a platform, so it makes sense to just make a dedicated binding and interop library that can be used anywhere.

### Support

Documentation for libsa can be found at https://stirlinglabs.github.io/libsa

Developed by [Stirling Labs](https://stirlinglabs.github.io).
