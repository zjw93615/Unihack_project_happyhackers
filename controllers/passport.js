// load all the things we need
var LocalStrategy = require('passport-local').Strategy;
var dummy_patients = require('../models/dummyPatient.js');

// console.log(dummy_patients[0].patientID);

// load up the user model
var User = require('../models/user_model.js');

module.exports = function(passport) {

    // =========================================================================
    // passport session setup ==================================================
    // =========================================================================
    // required for persistent login sessions
    // passport needs ability to serialize and unserialize users out of session

    passport.serializeUser(function(user, done) {
        done(null, user.id);
    });

    // used to deserialize the user
    passport.deserializeUser(function(id, done) {
        User.findById(id, function(err, user) {
            done(err, user);
        });
    });

    // =========================================================================
    // LOGIN =============================================================
    // =========================================================================
    passport.use('local-login', new LocalStrategy({
        // by default, local strategy uses username and password, we will override with email
        usernameField : 'email',
        passwordField : 'password',
        passReqToCallback : true // allows us to pass in the req from our route (lets us check if a user is logged in or not)
    }, function(req, email, password, done) {
        if (email)
            email = email.toLowerCase(); // Use lower-case e-mails to avoid case-sensitive e-mail matching

            // asynchronous
            process.nextTick(function() {
                User.findOne({ 'email' :  email}, function(err, user) {
                    // if there are any errors, return the error
                    if (err)
                        return done(err);

                    // if no user is found, return the message
                    if (!user)
                        return done(null, false, req.flash('loginMessage', 'No user found.'));

                    if (!user.validPassword(password))
                        return done(null, false, req.flash('loginMessage', 'Oops! Wrong password.'));

                    // all is well, return user
                    else
                        return done(null, user);
                });
            });

        }));



    // // =========================================================================
    // // SIGNUP ============================================================
    // // =========================================================================
    passport.use('local-signup', new LocalStrategy({
            // by default, local strategy uses username and password, we will override with email
            usernameField : 'email',
            passwordField : 'password',
            passReqToCallback : true // allows us to pass in the req from our route (lets us check if a user is logged in or not)
        },
        function(req, email, password, done) {
            // asynchronous
            process.nextTick(function() {
                // if the user is not already logged in:
                if (!req.user) {
                    User.findOne({ 'email': email}, function(err, user) {
                        // if there are any errors, return the error
                        if (err)
                            return done(err);

                        // check to see if theres already a user with that email
                        if (user) {
                            console.log("have taken");
                            return done(null, false, req.flash('signupMessage', 'That username is already taken.'));
                        } else {

                            // create the user
                            var newUser      = new User();
                            // var dummyPatient = [
                            //     {"patientID": "p1"}, 
                            //     {"patientID": "p2"},
                            //     {"patientID": "p3"}, 
                            //     {"patientID": "p4"}, 
                            //     {"patientID": "p5"}, 
                            //     {"patientID": "p6"}, 
                            //     {"patientID": "p7"}, 
                            //     {"patientID": "p8"}, 
                            //     {"patientID": "p9"}, 
                            //     {"patientID": "p10"}, 
                            // ];
                            var dummyPatient = dummy_patients;

                            newUser.email    = email;
                            newUser.name     = req.body.name;
                            newUser.phone    = req.body.phone;
                            newUser.password = newUser.generateHash(password);
                            newUser.patients = dummyPatient;

                            newUser.save(function(err, save) {
                                if (err)
                                    return done(err);
                                return done(null, newUser);
                            });
                        }

                    });
                    // if the user is logged in but has no local account...
                }else {
                    console.log("have talogken");
                    // user is logged in and already has a local account. Ignore signup. (You should log out before trying to create a new account, user!)
                    return done(null, req.user);
                }

            });

        }));

};