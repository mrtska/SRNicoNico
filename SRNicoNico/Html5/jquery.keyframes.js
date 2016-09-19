/*!
 * jQuery Keyframes v1.2.1
 *
 * Copyright 2016 SDN Project
 * MIT license
 *
 * https://sdn-project.net
 * https://sdn-project.net/m/jquery_keyframes
 */

(function ($) {
  var
    style = $('<style/>').appendTo('head')[0].sheet,
    isObj = $.isPlainObject,
    isFunc = $.isFunction,
    isNum = $.isNumeric;

  function createKey() {
    var
      str = 'abcdefghijklmnopqrstuvwxyz0123456789',
      len = str.length,
      key = 'keyframes-',
      i = 0;

    for (; i < 10; i++) {
      key += str[Math.floor(Math.random() * len)];
    }

    return key;
  }

  function toChainCase(str) {
    return str.replace(/[A-Z]/g, function (str) {
      return '-' + str.charAt(0).toLowerCase();
    });
  }

  function resetAnimation(elm) {
    var data = elm[0].keyframes;

    if (data.fill) {
      return;
    }

    $.each(style.cssRules, function (i, rule) {
      if (rule.name === data.key) {
        data.key = '';

        elm.prop({
          animated: false,
          paused: false
        }).css({
          animation: '',
          animationPlayState: ''
        }).off('animationiteration animationend');

        style.deleteRule(i);

        return false;
      }
    });
  }

  $.fn.keyframes = function (keys, opt, callback) {
    return this.each(function () {
      var
        that = this,
        $this = $(that),
        data = that.keyframes,
        iteration,
        key,
        steps,
        set;

      opt = opt || {};

      if (!isObj(data)) {
        that.animated = false;
        that.paused = false;

        data = {
          key: '',
          fill: false
        };
        that.keyframes = data;
      }

      if (that.animated || that.paused || data.fill) {
        if (keys === 'toggle') {
          keys = !that.paused;
        }
        if (keys === false || keys === 'run') {
          that.animated = true;
          that.paused = false;
          that.style.animationPlayState = 'running';
        } else if (keys === true || keys === 'pause') {
          that.animated = false;
          that.paused = true;
          that.style.animationPlayState = 'paused';
        } else {
          data.fill = false;
          resetAnimation($this);
        }
      }

      if (isObj(keys)) {
        if (isFunc(opt) || opt.iteration || opt.end) {
          callback = opt;
        } else if (isNum(opt)) {
          opt = {
            duration: opt
          };
        }

        if (isObj(callback)) {
          iteration = callback.iteration;
          callback = callback.end;
        }

        key = createKey();
        opt = isObj(opt) ? opt : {};
        data.fill = /^[bf]o/.test(opt.fill);

        that.style.animation = [
          key,
          (opt.duration || 400) + 'ms',
          opt.easing || 'ease-in-out',
          (opt.delay || 0) + 'ms',
          opt.count || 1,
          opt.direction || 'normal',
          opt.fill || 'none'
        ].join(' ');

        steps = Object.keys(keys);

        if (!/^(from|to|\d+%(, ?\d+%)*)$/.test(steps[0])) {
          keys = {
            to: keys
          };
          steps = ['to'];
        }

        set = '@keyframes ' + key + '{';
        steps.forEach(function (per) {
          var value = [];

          set += per + '{';
          $.each(keys[per], function (prop, val) {
            if (/^(matrix|translate|rotate|scale|skew)/.test(prop)) {
              if (isNum(val)) {
                if (/^trans/.test(prop)) {
                  val += 'px';
                } else if (!/^scale/.test(prop)) {
                  val += 'deg';
                }
              }

              value.push(prop + '(' + val + ')');
              return;
            }

            if (!/^(op|lin|z)/.test(prop) && isNum(val)) {
              val += 'px';
            }

            set += toChainCase(prop) + ':' + val + ';';
          });

          if (value.length > 0) {
            set += 'transform:' + value.join(' ') + ';';
          }
          set += '}';
        });
        set += '}';

        style.insertRule(set, style.cssRules.length);

        $this.on({
          animationiteration: function () {
            if (isFunc(iteration)) {
              iteration.call(that);
            }
          },
          animationend: function () {
            resetAnimation($this);

            if (isFunc(callback)) {
              callback.call(that);
            }
          }
        });

        data.key = key;
        that.animated = true;
      }
    });
  };
})(jQuery);
