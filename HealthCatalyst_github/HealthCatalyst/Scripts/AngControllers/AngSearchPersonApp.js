    
var appModule = angular.module('SearchApp', []);

appModule.controller('SearchPersonsCntrl', ['$scope', '$http', '$window',
        function ($scope, $http, $window) {
             // Initialize Controller during page load
            $scope.msg = ' ';
            $scope.error = ' ';
            $scope.isResult = false;
            $scope.jsonArgs = "";

            // These options are for the spinner
            $scope.opts = {
                lines: 12            // The number of lines to draw
              , length: 15           // The length of each line
              , width: 7             // The line thickness
              , radius: 7            // The radius of the inner circle
              , scale: 1             // Scales overall size of the spinner
              , corners: 1           // Corner roundness (0..1)
              , color: '#rrggbb'     // #rgb or #rrggbb or array of colors
              , opacity: 0.25        // Opacity of the lines
              , rotate: 40           // The rotation offset
              , direction: 1         // 1: clockwise, -1: counterclockwise
              , speed: 1.0           // Rounds per second
              , trail: 60            // Afterglow percentage
              , fps: 20              // Frames per second when using setTimeout() as a fallback for CSS
              , zIndex: 2e9          // The z-index (defaults to 2000000000)
              , className: 'spinner' // The CSS class to assign to the spinner
              , top: '50%'           // Top position relative to parent
              , left: '50%'          // Left position relative to parent
              , shadow: true         // Whether to render a shadow
              , hwaccel: false       // Whether to use hardware acceleration
              , position: 'absolute' // Element positioning
            }
            $scope.spinnerTarget = document.getElementById('spinner')
            $scope.spinner = new Spinner($scope.opts);

            // called from form submit
            $scope.GetSearchResults = function () {
                $scope.msg = ' ';
                $scope.error = ' ';
                $scope.isResult = false;

                // if the search argument is blank - force user to enter something before proceeding any further
                if ($scope.searchCriteria == undefined || $scope.searchCriteria.toString().trim() == '') {
                    $scope.error = 'Please enter a full or partial name';
                    $('#searchCriteria').focus();
                    return;
                }

                $scope.jsonObj = JSON.stringify({ Name: $scope.searchCriteria });

                // start the 'working' spinner
                $scope.startSpinner();
 
                //simulate a wait for results set
                $window.setTimeout($scope.CallSearchSvc, 2500);
            }; // end getSearchResults

            $scope.CallSearchSvc = function () {
                $('#searchCriteria').focus();
                $http.post('/Person/SearchPersons', $scope.jsonObj)
                     .success(function (results) {
                         $scope.successPost(results);
                     })
                     .error(function (results) {
                         $scope.errorPost(results);
                     });
            }; // end callSearchSvc

            // evaluate success from Http call
            $scope.successPost = function (results) {
                try {
                    if (results.Error != undefined) {
                        $scope.error = results.Error;
                        $scope.stopSpinner();
                        return;
                    }
                    if (results.NotFound != undefined) {
                        $scope.error = 'There were no results found for this request';
                        $scope.stopSpinner();
                        return;
                    }
                }
                catch (Exception) {
                    var errMsg = Exception.message;
                    $scope.error = 'There was an error parsing the JSON returned from the host=>' + errMsg;
                    $scope.stopSpinner();
                    return;
                }

                // results were present - display them
                $scope.isResult = true;

                // clear last results
                $scope.Persons = [];

                // render new results
                $scope.Persons = results;
                if ($scope.Persons.length != undefined) {
                    $scope.msg = 'Found ' + $scope.Persons.length + ' search results';
                }
                $scope.stopSpinner();
            };
            
            // evaluate error from Http call
            $scope.errorPost = function (result) {
                $scope.error = 'There was an error during POST to server=>' + result;
                stopSpinner();
                $scope.$apply();
            };

            $scope.startSpinner = function () {
                $scope.spinner.spin($scope.spinnerTarget);
            };

            $scope.stopSpinner = function () {
                $scope.spinner.stop();
            };

        }
    ]);
