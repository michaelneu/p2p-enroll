# P2P.Enroll

[![Build Status](https://travis-ci.org/michaelneu/p2p-enroll.svg?branch=master)](https://travis-ci.org/michaelneu/p2p-enroll)

This repo contains a sample implementation for the project enrollment of TUM's P2P systems and security course.

In order to register for the final project, a TCP connection had to be established. After reading a challenge, a nonce had to be found, so that the SHA256 hash of the message containing the challenge and the nonce matched a certain difficulty.


## Benchmark

When building this project in Release mode and running it from the command line, it can mine nonces with ~6.4 MH/s on an idle Intel Xeon Prozessor E3-1231 v3 at 3.4 GHz with one thread per logical core under Windows 10.


## License

The code in this repository is released under the [GPLv3](LICENSE).
