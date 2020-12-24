# Booma.Server

Booma.Server is a C#/.NET Phantasy Star Online: Blue Burst server emulator backend implementation based on the [Booma emulation library called Booma.Proxy](https://github.com/helloKitty/booma.proxy).

Utilizing [GladNet4](https://github.com/HelloKitty/GladNet3/tree/gladnet4) and the powerful [FreecraftCore.Serializer](https://github.com/FreecraftCore/FreecraftCore.Serializer) allows the project to be written productively and at a high-level when compared to *traditional* emulation.

## Services

### Patch

**[Patch Service](https://github.com/HelloKitty/Booma.Server/tree/master/src/Booma.Server.PatchService)**: TCP Server Application that defaults to running on port 11000. Protocol based on [Booma.Packet.Patch](https://github.com/HelloKitty/Booma.Proxy/tree/master/src/Booma.Packet.Patch). See the [documentation](https://github.com/HelloKitty/Booma.Proxy/blob/master/docs/PatchPacketDocumentation.md) for valid message/packet types.

This service is unfortunately low priority and of low usefulness and so minimal emulation for the PSOBB patching protocol will be implemented. It's suggested a non-emulated patching solution be utilized for patching the client instead.


## Credits

This project is built on top of **20 years** of reverse engineering work done by many in the PSO community. Much was learned and owed to some of the following projects such as: Sodaboy's proxy, [Sylverant's opensource C++ DC/BB/GC server implementation](https://github.com/Sylverant/) and one of the [most recent public Tethella releases](https://github.com/justnoxx/psobb-tethealla/).

Special thanks to @Soly for implementing and explaining the cryptography involved and his continued contributions to the underlying library that powers this server emulator found at: [Booma.Proxy](https://github.com/helloKitty/booma.proxy)

A complete list of people who helped make this possible can be found here: [Contributors.md](https://github.com/HelloKitty/Booma.Proxy/blob/master/Contributors.md)

## License

Contributions including pull requests, commits, notes, dumps, gists or anything else in the repository are licensed under the below licensing terms.

AGPL 3.0 with a seperate unrestricted, non-exclusive, perpetual, and irrevocable license also granted to [Andrew Blakely](https://www.github.com/HelloKitty)
