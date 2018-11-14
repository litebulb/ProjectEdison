//Init
var directLine;
var users = [];
var fromUser = {};
var connectionInfo;

$(document).ready(function() {
	/*
	BREAK auto refresh
	function saveConnectionInfo(connectionInfo){
		localStorage.setItem("token", connectionInfo.token );
		localStorage.setItem("streamUrl", connectionInfo.streamUrl );
		localStorage.setItem("conversationId", connectionInfo.conversationId );
		localStorage.setItem("displayName", connectionInfo.userContext.displayName );
		localStorage.setItem("userId", connectionInfo.userContext.userId );
	}
	
	function loadConnectionInfo(){
		var connectionInfo = {};
		connectionInfo.userContext = {};
		connectionInfo.token = localStorage.getItem("token");
		connectionInfo.streamUrl = localStorage.getItem("streamUrl");
		connectionInfo.conversationId = localStorage.getItem("conversationId");
		connectionInfo.userContext.displayName = localStorage.getItem("displayName");
		connectionInfo.userContext.userId = localStorage.getItem("userId");
		return connectionInfo;
	}*/
	
	function start(connectionInfo){
		fromUser.name = connectionInfo.userContext.name;
		fromUser.id = connectionInfo.userContext.id;
		fromUser.role = connectionInfo.userContext.role;
		fromUser.properties = {};
		fromUser.bearer = bearer;
		console.log(fromUser);
		
		//Initiate connection
		directLine = new DirectLine.DirectLine({
			token: connectionInfo.token,
			//streamUrl : connectionInfo.streamUrl,
			//conversationId: connectionInfo.conversationId,
			watermark: 0 //,
			///webSocket: false
		});
		
		//Add subscription on messages
		directLine.activity$
		.filter(activity => activity.type === 'message' && activity.text != null && activity.text != ''&& activity.channelData != null)
		.subscribe(
			activity => {
				//console.log(activity);
				
			}
		);
		//Add subscription on commands
		directLine.activity$
		.filter(activity => activity.type === 'message')
		.subscribe(
			activity => {
				if(activity.channelData.baseCommand == 'SendMessage'){
					
					console.log(activity);
					if(activity.channelData.data.from.role != 'Admin' && !users.includes(activity.channelData.data.from.id)){
						addUserTab(activity.channelData.data);
						users.push(activity.channelData.data.from.id);
					}
					var messages = document.getElementById("messages");
					var newcontent = document.createElement('div');
					newcontent.innerHTML = activity.timestamp + " " + activity.channelData.data.from.name + ": " + activity.text;
					messages.appendChild(newcontent);
				} 
				else if(activity.channelData.baseCommand == 'ReadUserMessages'){
					console.log('ReadUserMessages');
					console.log(activity);
				}
				else if(activity.channelData.baseCommand == 'EndConversation'){
					console.log('EndConversation');
					console.log(activity);
				}
			}
		);
		
		directLine.connectionStatus$
		.subscribe(connectionStatus => {
			if(connectionStatus == 2){
				console.log("connected");
				let cmdGetTranscript = {};
				cmdGetTranscript.basecommand = 'getTranscript';
				postActivity(fromUser, cmdGetTranscript, "", p => {
					console.log("transcript received");
				});
			}
		});
	}

	//Init
	//connectionInfo = loadConnectionInfo();
	//if(connectionInfo == null || connectionInfo.token == null){
		$.ajax(
		{
			url: "https://edisonapidev.eastus.cloudapp.azure.com/chat/security/gettoken/",
			type: 'GET',
			headers: {"Authorization": "Bearer " + bearer  }, 
			success: function( data ) {
				var connectionInfo = data;
				console.log(connectionInfo);
				//saveConnectionInfo(connectionInfo);
				start(connectionInfo);
			}, 
			error: function (error){
				console.log(error);
			}
		}
		);
	//} else {
	//	start(connectionInfo);
	//}
});







