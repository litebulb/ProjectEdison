function post(){
	let commandObj = {}
	commandObj.baseCommand = 'SendMessage';
	commandObj.data = {};
	commandObj.data.from = {};
	commandObj.data.from.id = fromUser.id;
	commandObj.data.from.name = fromUser.name;
	commandObj.data.from.role = fromUser.role;
	commandObj.data.userId = document.getElementById("recipient").value;
	
	var input = document.getElementById("input").value;
	postActivity(fromUser, commandObj, input);
}

function readstatus(){
	let commandObj = {}
	commandObj.baseCommand = 'ReadUserMessages';
	commandObj.data = {};
	commandObj.data.userId = document.getElementById("recipient").value;
	commandObj.data.date = parseInt(new Date().getTime() / 1000)
	
	var input = document.getElementById("input").value;
	postActivity(fromUser, commandObj, input);
}

function endconversation(){
	let commandObj = {}
	commandObj.baseCommand = 'EndConversation';
	commandObj.data = {};
	commandObj.data.userId = document.getElementById("recipient").value;
	
	var input = document.getElementById("input").value;
	postActivity(fromUser, commandObj, input);
}

function addUserTab(recipient){
	//console.log(recipient);
	var users = document.getElementById("users");
	var newcontent = document.createElement('div');
	var link = document.createElement('a');
	link.href = 'javascript:changeUser(\'' + recipient.from.id + '\');';
	link.innerHTML = recipient.from.name;
	newcontent.appendChild(link);
	users.appendChild(newcontent);
}

function changeUser(userId){
	document.getElementById("recipient").value = userId;
}

function postActivity(fromUser, channelObj = null, message = null, callback = null){
	directLine.postActivity({
		from: { id: fromUser.id, name: fromUser.name, UserToken: fromUser.UserToken }, // required (from.name is optional)
		type: 'message',
		channelData: channelObj == null ? null : channelObj, //JSON.stringify(channelObj),
		text: message
	}).subscribe(
		id => {
			console.log("Posted activity, assigned ID ", id);
			if(callback != null)
				callback(id);
		},
		error => console.log("Error posting activity", error)
	);
}