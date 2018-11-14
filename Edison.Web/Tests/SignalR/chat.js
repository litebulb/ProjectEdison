// The following sample code uses modern ECMAScript 6 features 
// that aren't supported in Internet Explorer 11.
// To convert the sample for environments that do not support ECMAScript 6, 
// such as Internet Explorer 11, use a transpiler such as 
// Babel at http://babeljs.io/. 
//
// See Es5-chat.js for a Babel transpiled version of the following code:

const tokenValue = 'USER_SECRET';
var connection = null;


function connectSignalR(){
	//signalR.HttpTransportType.WebSockets not working??
    //https://edisonapidev.eastus.cloudapp.azure.com/signalr
	connection = new signalR.HubConnectionBuilder()
		.withUrl("http://localhost:51412/signalr",{ () => tokenValue})
		.configureLogging(signalR.LogLevel.Information)
		.build();
	//connection.qs = { 'Token' : tokenValue};
	//connection.serverTimeoutInMilliseconds = 5000; // 5 second


    connection.on("UpdateEventClusterUI", (message) => {
        console.log('Receiving UpdateEventClusterUI...');
        console.log(message);
    });

	connection.on("UpdateResponseUI", (message) => {
        console.log('Invoking UpdateResponseUI...');
		console.log(message);
    });

    connection.on("UpdateDeviceUI", (message) => {
        console.log('Invoking UpdateDeviceUI...');
        console.log(message);
    });

    connection.on("UpdateChatUI", (message) => {
        console.log('Invoking UpdateChatUI...');
        console.log(message);
    });

	connection.start().catch(err => console.error(err.toString()));
}

connectSignalR();