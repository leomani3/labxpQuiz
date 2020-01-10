var express = require('express');
var session = require('express-session');
var app = express();

var session = require("express-session")({
    secret: "my-secret",
    resave: true,
    saveUninitialized: true
});
var sharedsession = require("express-socket.io-session");

var lineReader = require('readline').createInterface({
  input: require('fs').createReadStream('Server/Questions.txt')
});

app.use(session);



// la base
var io = require('socket.io')({
	transports: ['websocket'],
});

io.use(sharedsession(session, {
    autoSave:true
}));

io.attach(process.env.PORT);
console.log(process.env.PORT);


// include our files
var tools = require('Server/Game');
tools.Main(io);


global.playerNumber = 0;
global.numberQuestion = 0;
global.nbPlayerAnswer = 0;

global.scorePlayer = new Map();
global.namePlayerMap = new Map();
global.nbJoueurGetScore=0;

console.log("sa tourne au calme");

i =0;
global.questions = [];

lineReader.on("line", function (line) {	
	question = {};

	answer = {};
	questionAndResponses = line.split("//")[0];
	indexResponse = line.split("//")[1];
	
	questionTitle = questionAndResponses.split("#&")[0];
	responses = questionAndResponses.split("#&")[1];
	responsesString = responses.split("/");
	for(j = 0 ; j < responsesString.length ; j++){
		response = responses.split("/")[j];
		answer[j] =  response;
	}
	
	question = {"title" : questionTitle,"goodAnswer" : indexResponse ,answer };
	
	questions.push(question);
	i++;
});



