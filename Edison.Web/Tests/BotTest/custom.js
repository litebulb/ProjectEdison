function post(){
	var input = document.getElementById("input").value;
	console.log(input);
	directLine.postActivity({
		from: { id: 'admin1@edisonadmin.com', name: 'Emergency Team' }, // required (from.name is optional)
		type: 'message',
		text: input
	}).subscribe(
		id => console.log("Posted activity, assigned ID ", id),
		error => console.log("Error posting activity", error)
	);
}

var directLine = new DirectLine.DirectLine({
    secret: "Jd0VvD2-UeI.cwA.YnI.SnbKnQ2EjfYSkVN-naqej1b6xnFpx-WEOISCvcQsgvY",
    domain: "",
    webSocket: true
});

directLine.activity$
.filter(activity => activity.type === 'message' && activity.from.id === 'edisonconversationagentdev')
.subscribe(
    message => {
		console.log(message);
		var messages = document.getElementById("messages");
		var newcontent = document.createElement('div');
		newcontent.innerHTML = message.text;
		
		
		//while (newcontent.firstChild) {
        messages.appendChild(newcontent);
		//}
	}
);