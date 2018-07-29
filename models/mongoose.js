// Establish DB

var mongoose = require('mongoose');
mongoose.connect('mongodb://admin:happyhackers8@ds257981.mlab.com:57981/unihack_project', function(err){
    if(!err){
        console.log('Connected to mongo');
    }else{
        console.log('Failed to connect to mongo');
    }
});
module.exports = mongoose;
