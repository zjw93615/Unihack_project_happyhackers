// Init App
var express = require('express');
var app = express();

require('./models/mongoose')

// Passport Init
var passport = require('passport');
var flash = require('connect-flash');

var morgan = require('morgan');
var cookieParser = require('cookie-parser');
var bodyParser = require('body-parser');
var session = require('express-session');

require('./models/mongoose.js');
require('./controllers/passport')(passport);

app.use(morgan('dev')); // log every request to the console
app.use(cookieParser());
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({
    extended: true
}));

// Express Session
app.use(session({
    secret: 'this-is-a-secret-token',
    resave: true,
    saveUninitialized: true,
    maxAge: 600000,
}));

app.use(passport.initialize());
app.use(passport.session());
app.use(flash());

module.exports.passport = passport;

// View Engine
app.set('view engine', 'ejs');

// Set Static Folder
app.use(express.static(__dirname + '/public'));

// Router
var router = require('./routes/router');
app.use(router);

// 404 page
app.use(function (req, res, next) {
    res.status(404);
    if (req.accepts('html')) {
        res.render('404', {
            url: req.url
        });
    }
});

// Set Portno
var PORT = process.env.PORT || 8080;
app.listen(PORT);
console.log('Express listening on port ' + PORT);



// create web socket server
const WebSocket = require('ws');
const wss = new WebSocket.Server({
    port: 8000
}, () => {
    console.log('listening on 8000');
});

var gameWs;

wss.on('connection', function connection(ws) {
    console.log("CONNECTION");
    console.log(ws);
    gameWs = ws;


    /* /start
     * /stop 
     * game-id should be the index of the client in the list
     */
    ws.on('message', function incoming(message) {
        // console.log('received: ' + message);
        jsonObj = JSON.parse(message);
        console.log(jsonObj);
        if (Array.isArray(jsonObj)) {
            // Check each item, real time object
            for (item in jsonObj) {
                if ("posX" in jsonObj[item]) { // posX posY together here
                    // Render posX value here
                }
                if ("stepHeight" in jsonObj[item]) {
                    // Render stepHeight value here
                }
                if ("stepTime" in jsonObj[item]) {
                    // Render stepTime value here
                }
            }
            console.log(jsonObj);
        } else {
            // Final result, It is a JSON object
            gameWs.send("received");
        }
    });

    gameWs.send("Hello David");

});


var events = require('events');
var eventEmitter = new events.EventEmitter();

// listener #1 for start game
var listner1 = function listner1() {
    wss.clients.forEach((client)=> {
        client.send("start");

    });
 }

 // listener #2 for stop game
 var listner2 = function listner2() {
    wss.clients.forEach((client)=> {
        client.send("stop");
    });
 }

 // Bind the connection event with the listner1 function
eventEmitter.addListener('startGame', listner1);
eventEmitter.addListener('stopGame', listner2);

var eventListeners = require('events').EventEmitter.listenerCount
   (eventEmitter,'connection');
console.log(eventListeners + " Listner(s) listening to connection event");

module.exports = {
    // sendMessage :function (msg, callback) {
    //     return wss.on("connection", function (ws) {
    
    //         ws.send(msg, callback);
    
    
    //         ws.on("close", function () {
    //             console.log("websocket connection close")
    //         })
    //     })
    // },
    start: function() {
        eventEmitter.emit('startGame');
    },
    stop: function() {
        eventEmitter.emit('stopGame');
    } 
}


