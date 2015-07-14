(function () {
    'use strict';

    var gulp = require('gulp'),
        runSequence = require('run-sequence'),
        concatCss = require('gulp-concat-css'),
        uglify = require('gulp-uglify'),
        concat = require('gulp-concat'),
        minifyCss = require('gulp-minify-css');

    gulp.task('css', function () {
        return gulp.src(['Content/**/*.css', 'font-awesome/css/font-awesome.css'])
            .pipe(concatCss("all.css"))
            .pipe(minifyCss())
            .pipe(gulp.dest('Content'));
    });

    gulp.task('js', function () {
        return gulp.src(['Scripts/app/**/*.js', 'Scripts/plugins/ladda/*.js',
             'Scripts/plugins/dataTables/*.js', 'Scripts/plugins/metisMenu/*.js',
            'Scripts/plugins/morris/*.js', 'Scripts/plugins/flot/*.js', 'Scripts/widget-engine.js'])
          .pipe(concat("fb.js"))
          .pipe(uglify())
          .pipe(gulp.dest('Scripts'));
    });

    gulp.task('default', function (callback) {
        runSequence('css', 'js', callback);
    });

})();