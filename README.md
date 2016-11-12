# MultipleClientServer

On the basis of the [SocketAsyncEventArgs](https://msdn.microsoft.com/en-us/library/system.net.sockets.socketasynceventargs.aspx) class and the presented sample code, this application came into existence as a proof of concept for high performance TCP/IPv4 socket code.

## Features

* Connect to multiple application instances
* Sending text messages
* Sending big files (>2.15GB)

The default port is 51010.

## Caution

This is the first attempt (of the author) of writing more complex socket code so there may still exist undetected errors because no elaborate testing was performed. For now functionality has only been tested within a local area network.

## Use

1. Connect to another client by entering hte IPv4 address and clicking the CONNECT button
2. Select file and/or enter text in the respective field
3. Initiate data transfer by clicking the SEND button
4. Disconnect from client by clicking the respective DISCONNECT button in the tab page.

## Further reading

* [Microsoft](https://msdn.microsoft.com/en-us/library/system.net.sockets.socketasynceventargs.aspx) - SocketAsyncEventArgs class
* [Stan Kirk](http://www.codeproject.com/Articles/83102/C-SocketAsyncEventArgs-High-Performance-Socket-Cod) - C# SocketAsyncEventArgs High Performance Socket Code
* [Marcos Hidalgo Nunes](http://www.codeproject.com/Articles/22918/How-To-Use-the-SocketAsyncEventArgs-Class) - How To Use the SocketAsyncEventArgs Class

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
