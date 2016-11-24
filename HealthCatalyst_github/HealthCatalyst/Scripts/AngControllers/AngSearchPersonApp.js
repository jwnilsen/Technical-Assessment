    
var appModule = angular.module('SearchApp', []);

appModule.controller('SearchPersonsCntrl', ['$scope', '$http', '$window',
        function ($scope, $http, $window) {
             // Initialize Controller during page load
            $scope.msg = ' ';
            $scope.error = ' ';
            $scope.isResult = false;
            //$('#divPersonList').hide();
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

            $scope.localjsonObjs = [
                { "Name": "Sanders, John", "Address": "Salt Lake City", "Age": "5", "Interests": "None" },
                { "Name": "Willsey, Bob", "Address": "West Valley", "Age": "63", "Interests": "Too Many" },
                { "Name": "Moore, Mandy", "Address": "Provoy", "Age": "31", "Interests": "Life" }];
            //alert("1st=>" + $scope.fakeResults[0].toString() + "2nd=>" + $scope.fakeResults[1].toString()) ;

            // called from form submit
            $scope.GetSearchResults = function () {
                $scope.msg = ' ';
                $scope.error = ' ';
                $scope.isResult = false;
                //$('#divPersonList').hide();

                // add dummy entry
                //$scope.Persons = [{ "Name": "", "Address": "", "Age": "", "Interests": "" }];

                // if the search argument is blank - force user enter something before proceeding any further
                if ($scope.searchCriteria == undefined || $scope.searchCriteria.toString().trim() == '') {
                    $scope.error = 'Please enter a full or partial name';
                    $('#searchCriteria').focus();
                    return;
                }

                $scope.jsonObj = JSON.stringify({ Name: $scope.searchCriteria });

                // start the 'working' spinner
                $scope.startSpinner();

                //$('#divPersonList').show();
                //$scope.Persons = $scope.fakeResults;
                //stopSpinner();
                //return;
 
                //simulate a wait for results set
                $window.setTimeout($scope.CallSearchSvc, 2500);
            }; // end getSearchResults

            $scope.CallSearchSvc = function () {
                $('#searchCriteria').focus();
                // call host service to see if there are any matching database entries
                $.ajax({ 
                    url: '/Person/SearchPersons',
                    type: 'POST',
                    data: $scope.jsonObj,
                    contentType: "application/json",
                    success: function (results) {
                        $scope.successPost(results)
                    },
                    error: function (results) {
                        $scope.errorPost(results)
                    }
                });
            }; // end callSearchSvc

            // evaluate success from Http call
            $scope.successPost = function (results) {
                // using this because of AJAX call
                // need it to be aware of any additional $scope variable modifications
                $scope.$apply( function () {
                    try {
                        // convert JSON string into JSON objects
                        $scope.jsonObjs = JSON.parse(results);
                        if ($scope.jsonObjs.Error != undefined) {
                            //$('#lblErrorMsg').text(result.Error);
                            $scope.error = jsonObjs.Error;
                            $scope.stopSpinner();
                            //$scope.$apply();
                            return;
                        }
                        if ($scope.jsonObjs.NotFound != undefined) {
                            //$('#lblErrorMsg').text('There were no results found for this request');
                            $scope.error = 'There were no results found for this request';
                            $scope.stopSpinner();
                            //$scope.$apply();
                            return;
                        }
                    }
                    catch (Exception) {
                        var errMsg = Exception.message;
                        //$('#lblErrorMsg').text('There was an error parsing the JSON returned from the host');
                        $scope.error = 'There was an error parsing the JSON returned from the host';
                        $scope.stopSpinner();
                        //$scope.$apply();
                        return;
                    }

                    // results were present - display them
                    $scope.jsonObjs = JSON.parse(results);
                    $scope.isResult = true;
                    //$('#divPersonList').show();

                    // clear last results
                    $scope.Persons = [];

                    // render new results
                    $scope.Persons = $scope.jsonObjs;
                    //$scope.Persons = $scope.localjsonObjs;

                    if ($scope.Persons.length != undefined)
                    {
                        $scope.msg = 'Found ' + $scope.Persons.length + ' search results';
                    }
                    $scope.stopSpinner();
                    //$scope.$apply();
                })
            };
            
            // evaluate error from Http call
            $scope.errorPost = function (result) {
                $scope.error = 'There was an error parsing the JSON returned from the host';
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
