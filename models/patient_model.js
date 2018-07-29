var mongoose = require('mongoose');

var patientSchema = mongoose.Schema({
    patientID: {
        type: String,
        required: true,
    },
    name: String,
    age: Number,
    sex: String,
    isPlaying: {
        type: Boolean,
        required: true
    },
    description: String,
    games:[
        {gameID: String}
    ],
    analysis: {
        average_scores: [
            {score: Number, date: Date}
        ], 
        average_steps: [
            {step: Number, date: Date}
        ],
        history_scores: [
            {score: Number, date: Date}
        ],
        history_dates: [
            {date: Date}
        ],
        history_steps: [
            {step: Number, date: Date}
        ],
        goal: Number, 
        state: String,
    }
});

module.exports = mongoose.model("Patient", patientSchema);