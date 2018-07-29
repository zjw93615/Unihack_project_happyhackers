var mongoose = require('mongoose');
var bcrypt = require('bcrypt-nodejs');

var userSchema = mongoose.Schema({
    email: {
        type: String,
        required: true
    },
    name: {
        type: String,
        required: true,
        unique: true
    },
    phone: String,
    password: {
        type: String,
        required: true
    },
    patients: [
        {
            patientID: String,
            name: String,
            age: Number,
            sex: String,
            isPlaying: Boolean,
            description: String,
            analysis: {
                average_scores: [
                    {
                        score: Number,
                        date: Date
                    }
                ], 
                average_steps: [
                    {
                        step: Number,
                        date: Date
                    }
                ],
                history_scores: [
                    {
                        score: Number,
                        date: Date
                    }
                ],
                history_dates: [
                    {
                        date: Date
                    }
                ],
                history_steps: [
                    {
                        step: Number,
                        date: Date
                    }
                ],
                coordinates: [
                    {
                        posX: Number, 
                        posY: Number
                    }
                ],
                goal: Number, 
                state: String,
            }

        }
    ]
});

// generating a hash
userSchema.methods.generateHash = function(password) {
    return bcrypt.hashSync(password, bcrypt.genSaltSync(8), null);
};

// checking if password is valid
userSchema.methods.validPassword = function(password) {
    return bcrypt.compareSync(password, this.password);
};

module.exports = mongoose.model('Users', userSchema);