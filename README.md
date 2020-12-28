# Booma.Server

Booma.Server is a C#/.NET Phantasy Star Online: Blue Burst server emulator backend implementation based on the [Booma emulation library called Booma.Proxy](https://github.com/helloKitty/booma.proxy).

Utilizing [GladNet4](https://github.com/HelloKitty/GladNet3/tree/gladnet4) and the powerful [FreecraftCore.Serializer](https://github.com/FreecraftCore/FreecraftCore.Serializer) allows the project to be written productively and at a high-level when compared to *traditional* emulation.

## Services

### Patch

**[Patch Service](https://github.com/HelloKitty/Booma.Server/tree/master/src/Booma.Server.PatchService)**: TCP Server Application that defaults to running on port 11000. Protocol based on [Booma.Packet.Patch](https://github.com/HelloKitty/Booma.Proxy/tree/master/src/Booma.Packet.Patch). See the [documentation](https://github.com/HelloKitty/Booma.Proxy/blob/master/docs/PatchPacketDocumentation.md) for valid message/packet types.

This service is unfortunately low priority and of low usefulness and so minimal emulation for the PSOBB patching protocol will be implemented. It's suggested a non-emulated patching solution be utilized for patching the client instead.

### Login

**[Login Service](https://github.com/HelloKitty/Booma.Server/tree/master/src/Booma.Server.LoginService)**: TCP Server Application that defaults to running on port 12000. Protocol based on [Booma.Packet.Game](https://github.com/HelloKitty/Booma.Proxy/tree/master/src/Booma.Packet.Game). See the [documentation](https://github.com/HelloKitty/Booma.Proxy/blob/master/docs/GamePacketDocumentation.md) for valid message/packet types.

This service is redirected to by the Patch Service and acts as a TCP endpoint for PSOBB clients to **initially** authenticate through. The process for logging into a TCP service on the PSOBB backend is the same across almost all services, this process is not exclusive to the Login Service. The client sends a [Login93](https://github.com/HelloKitty/Booma.Proxy/blob/00e5a01b62ebc97d15c2d62eee6d416464b867cf/src/Booma.Packet.Game/Shared/Payloads/Client/SharedLoginRequest93Payload.cs) packet after the [Welcome Packet](https://github.com/HelloKitty/Booma.Proxy/blob/3cb7d5de7acd241fd99d834222aa1aafa3df69e2/src/Booma.Packet.Game/Shared/Payloads/Server/SharedWelcomePayload.cs) from the service is sent. These two packets are critical to establishing a valid session from the server and PSOBB client. They initialize the crytography for the session and authenticate the user.

The Login Service exists only to validate the credentials of the user or send them back to the Titlescreen. If authentication is successful it will [Redirect](https://github.com/HelloKitty/Booma.Proxy/blob/00e5a01b62ebc97d15c2d62eee6d416464b867cf/src/Booma.Packet.Game/Shared/Payloads/Server/SharedConnectionRedirectPayload.cs) them to the Character Service. This login process happens across all services and the Login Service itself does not actually perform the authentication of the session. The Auth Service does this.

### Auth

**[Auth Service](https://github.com/HelloKitty/Booma.Server/tree/master/src/Booma.Server.AuthService)**: A stateless scalable ASP Core HTTP API for OAuth/JWT authentication and authorization. Based on the [Glader.ASP.Authentication](https://github.com/HelloKitty/Glader.ASP.Authentication) library.

This service is responible for actually authenticating a user based on credentials provided. Issues JWT (Java Web Tokens) for authorizing against other authorization required services. Services that require Authentication should send requests to this service. Login processses across all backend services depend on this service.

### Service Discovery

**[Service Discovery Service](https://github.com/HelloKitty/Booma.Server/tree/master/src/Booma.Server.ServiceDiscoveryService)**: A stateless scalable ASP Core HTTP API for discovering named services and routing. Based on the [Glader.ASP.ServiceDiscovery](https://github.com/HelloKitty/Glader.ASP.ServiceDiscovery) library.

This service is responible for allowing the backend services to discover eachother by service type/name. For example, the **Login Service** asks the **Service Discovery Service**  the endpoint for the **Auth Service** so it can perform authentication. It acts as the "Service Registry" as described by [NGINX's article Service Discovery in a Microservices Architecture](https://www.nginx.com/blog/service-discovery-in-a-microservices-architecture/).

## Credits

This project is built on top of **20 years** of reverse engineering work done by many in the PSO community. Much was learned and owed to some of the following projects such as: Sodaboy's proxy, [Sylverant's opensource C++ DC/BB/GC server implementation](https://github.com/Sylverant/) and one of the [most recent public Tethella releases](https://github.com/justnoxx/psobb-tethealla/).

Special thanks to @Soly for implementing and explaining the cryptography involved and his continued contributions to the underlying library that powers this server emulator found at: [Booma.Proxy](https://github.com/helloKitty/booma.proxy)

A complete list of people who helped make this possible can be found here: [Contributors.md](https://github.com/HelloKitty/Booma.Proxy/blob/master/Contributors.md)

## License

Contributions including pull requests, commits, notes, dumps, gists or anything else in the repository are licensed under the below licensing terms.

AGPL 3.0 with a seperate unrestricted, non-exclusive, perpetual, and irrevocable license also granted to [Andrew Blakely](https://www.github.com/HelloKitty)
