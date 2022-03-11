"use strict";

function asyncGeneratorStep(gen, resolve, reject, _next, _throw, key, arg) { try { var info = gen[key](arg); var value = info.value; } catch (error) { reject(error); return; } if (info.done) { resolve(value); } else { Promise.resolve(value).then(_next, _throw); } }

function _asyncToGenerator(fn) { return function () { var self = this, args = arguments; return new Promise(function (resolve, reject) { var gen = fn.apply(self, args); function _next(value) { asyncGeneratorStep(gen, resolve, reject, _next, _throw, "next", value); } function _throw(err) { asyncGeneratorStep(gen, resolve, reject, _next, _throw, "throw", err); } _next(undefined); }); }; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); Object.defineProperty(Constructor, "prototype", { writable: false }); return Constructor; }

function _defineProperty(obj, key, value) { if (key in obj) { Object.defineProperty(obj, key, { value: value, enumerable: true, configurable: true, writable: true }); } else { obj[key] = value; } return obj; }

var FeedbackWidget = /*#__PURE__*/function () {
  function FeedbackWidget(elementId) {
    _classCallCheck(this, FeedbackWidget);

    _defineProperty(this, "_key", "feedback_widget");

    this._elementId = elementId;
  }

  _createClass(FeedbackWidget, [{
    key: "elementId",
    get: function get() {
      //getter, set keyword voor setter methode
      return this._elementId;
    }
  }, {
    key: "show",
    value: function show(message, type) {
      $("#".concat(this._elementId)).text(message);
      var x = document.getElementById(this._elementId);
      x.className = "alert alert-".concat(type);
      x.style.display = "block";
      this.log({
        message: message,
        type: type
      });
    }
  }, {
    key: "hide",
    value: function hide() {
      var x = document.getElementById(this._elementId);
      x.style.display = "none";
    }
  }, {
    key: "log",
    value: function log(message) {
      var _JSON$parse;

      // haal alle feedback op, in geval van geen feedback nieuwe array
      var feedback = (_JSON$parse = JSON.parse(localStorage.getItem(this._key))) !== null && _JSON$parse !== void 0 ? _JSON$parse : []; // als er meer dan 10 feedback widgets aanwezig zijn gooi de eerste in de array weg

      if (feedback.length >= 10) feedback.splice(0, 1);
      feedback.push(message); // zet array van message objects om in JSON string format

      localStorage.setItem(this._key, JSON.stringify(feedback));
    }
  }, {
    key: "history",
    value: function history() {
      var messages = JSON.parse(localStorage.getItem(this._key));
      var text = "";
      messages.forEach(function (message) {
        text += "<type: ".concat(message.type, "> - <berichttekst:").concat(message.message, "> <\"\n\">");
      });
      this.show(text, "success");
    }
  }], [{
    key: "removeLog",
    value: function removeLog() {
      localStorage.removeItem(this._key);
    }
  }]);

  return FeedbackWidget;
}();

var Game = function (url) {
  //Configuratie en state waarden
  var _configMap = {
    apiUrl: url
  };
  var _stateMap = {
    gameState: ""
  };

  var _getCurrentGameState = function _getCurrentGameState() {
    setInterval(function () {
      Game.Model.getGameState(1); // de echte request moet nog komen

      _stateMap.gameState = ""; // deze moet nog naar correcte waarde geupdate worden
    }, 2000);
  };

  var _init = function _init(afterInit) {
    _getCurrentGameState();

    afterInit();
  };

  return {
    init: _init
  };
}('/api/url');

Game.Reversi = function () {
  var _configMap = {};

  var _init = function _init() {
    console.log(_configMap.apiUrl);
  };

  return {
    init: _init
  };
}();

Game.Data = function () {
  var _configMap = {
    apiKey: "aa6bb372c0ccba60aff08f3c0b3cf922",
    mock: [{
      url: "api/Spel/Beurt",
      data: 0
    }]
  };
  var _stateMap = {
    environment: "development"
  };

  var _init = function _init(environment) {
    if (environment !== "development" || environment !== "production") {
      throw new Error('This environment state does not exist (accepted states are: "production" and "development")');
    }

    this._stateMap.environment = environment;
  };

  var _getMockData = function _getMockData(url) {
    var mockData = _configMap.mock.filter(function (m) {
      return m.url === url;
    }).data;

    return new Promise(function (resolve, reject) {
      resolve(mockData);
    });
  };

  var _get = function _get(url) {
    if (_stateMap.environment === "development") {
      return _getMockData(url);
    }

    return $.get(url).then(function (r) {
      return r;
    })["catch"](function (e) {
      console.log(e.message);
    });
  };

  return {
    configmap: _configMap,
    init: _init,
    get: _get
  };
}();

Game.Model = function () {
  var _configMap = {};

  var _init = function _init() {};

  var _getWeather = function _getWeather() {
    var url = "http://api.openweathermap.org/data/2.5/weather?q=will&APPID=".concat(Game.Data.configMap.apiKey);
    Game.Data.get(url).then(function (data) {
      var _data$main;

      if (!(data !== null && data !== void 0 && (_data$main = data.main) !== null && _data$main !== void 0 && _data$main.temp)) {
        throw new Error("No temperature found");
      }

      console.log(data);
    })["catch"](function (e) {
      console.error(e.message);
    });
  };

  var _getGameState = /*#__PURE__*/function () {
    var _ref = _asyncToGenerator( /*#__PURE__*/regeneratorRuntime.mark(function _callee(token) {
      var state;
      return regeneratorRuntime.wrap(function _callee$(_context) {
        while (1) {
          switch (_context.prev = _context.next) {
            case 0:
              _context.next = 2;
              return Game.Data.get("/api/Spel/Beurt/".concat(token));

            case 2:
              state = _context.sent;

              if (!(state !== 0 && state !== 1 && state !== 2)) {
                _context.next = 5;
                break;
              }

              throw new Error("Geen valide state!");

            case 5:
              return _context.abrupt("return", state);

            case 6:
            case "end":
              return _context.stop();
          }
        }
      }, _callee);
    }));

    return function _getGameState(_x) {
      return _ref.apply(this, arguments);
    };
  }();

  return {
    init: _init,
    getWeather: _getWeather,
    getGameState: _getGameState
  };
}();