var mongoose = require('mongoose');

var gameSchema = mongoose.Schema({
    gameID: {
        type: String,
        required: true,
    },
    doctorID: {
        type: String,
        required: true
    },
    patientID: {
        type: String,
        required: true
    },
    timeLength: {
        type: Number,
        required: true
    },
    isStart: {
        type: Boolean,
        required: true,
        default: false
    },
    isFinish: {
        type: Boolean,
        required: true,
        default: false
    },
    timeStart: {
        type: Number,
        required: true
    },
    timeFinish: Number,
    actualTime: Number,
    difficulty: String,
    data: {
        score: {
            type: Number,
            required: true
        },
        steps: {
            type: Number,
            default: 0
        }
    }
});

module.exports = mongoose.model("Game", gameSchema);