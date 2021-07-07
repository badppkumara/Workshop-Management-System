'use strict';

(function ($) {
  'use strict';

  var App = {
    Constants: {
      CUSTOM_SCROLLBAR_DISTANCE: '4px',
      CUSTOM_SCROLLBAR_HEIGHT: '100%',
      CUSTOM_SCROLLBAR_SIZE: '7px',
      CUSTOM_SCROLLBAR_TOUCH_SCROLL_STEP: 50,
      CUSTOM_SCROLLBAR_WHEEL_STEP: 10,
      CUSTOM_SCROLLBAR_WIDTH: '100%',
      MEDIA_QUERY_BREAKPOINT: '992px',
      SELECT2_THEME: 'bootstrap',
      SELECT2_WIDTH: '100%',
      SHARE_MESSAGE_EXPIRES: 1,
      SHARE_MESSAGE_EXTENDED_TIME_OUT: 5000,
      SHARE_MESSAGE_KEY: 'share_message',
      SHARE_MESSAGE_PROGRESS_BAR: true,
      SHARE_MESSAGE_STATUS: true,
      SHARE_MESSAGE_TIME_OUT: 15000,
      STICKY_OFF_RESOLUTIONS: -768,
      STICKY_TOP: 55,
      TRANSITION_DELAY: 400,
      TRANSITION_DURATION: 400
    },
    CssClasses: {
      LAYOUT: 'layout',
      LAYOUT_HEADER: 'layout-header',
      LAYOUT_SIDEBAR: 'layout-sidebar',
      LAYOUT_CONTENT: 'layout-content',
      LAYOUT_FOOTER: 'layout-footer',

      LAYOUT_HEADER_FIXED: 'layout-header-fixed',
      LAYOUT_SIDEBAR_FIXED: 'layout-sidebar-fixed',
      LAYOUT_FOOTER_FIXED: 'layout-footer-fixed',

      LAYOUT_SIDEBAR_COLLAPSED: 'layout-sidebar-collapsed',
      LAYOUT_SIDEBAR_STICKY: 'layout-sidebar-sticky',

      SIDENAV: 'sidenav',
      SIDENAV_BTN: 'sidenav-toggler',
      SIDENAV_COLLAPSED: 'sidenav-collapsed',
      SIDENAV_ACTIVE: 'open',
      SIDENAV_COLLAPSE: 'collapse',
      SIDENAV_COLLAPSE_IN: 'in',
      SIDENAV_COLLAPSING: 'collapsing',

      SEARCH_FORM: 'navbar-search',
      SEARCH_FORM_BTN: 'navbar-search-toggler',
      SEARCH_FORM_COLLAPSED: 'navbar-search-collapsed',

      CUSTOM_SCROLLBAR: 'custom-scrollbar',
      CUSTOM_SCROLLBAR_BAR: 'custom-scrollbar-gripper',
      CUSTOM_SCROLLBAR_RAIL: 'custom-scrollbar-track',
      CUSTOM_SCROLLBAR_WRAPPER: 'custom-scrollable-area',

      STICKY: 'sticky-scrollbar',
      STICKY_WRAPPER: 'sticky-scrollable-area',

      THEME_PANEL: 'theme-panel',
      THEME_PANEL_BTN: 'theme-panel-toggler',
      THEME_PANEL_COLLAPSED: 'theme-panel-collapsed',

      COLLAPSED: 'collapsed'
    },
    KeyCodes: {
      S: 83,
      OPEN_SQUARE_BRACKET: 219,
      CLOSE_SQUARE_BRACKET: 221
    },
    init: function init() {
      this.$document = $(document);
      this.$layout = $('.' + this.CssClasses.LAYOUT);
      this.$header = $('.' + this.CssClasses.LAYOUT_HEADER);
      this.$sidebar = $('.' + this.CssClasses.LAYOUT_SIDEBAR);
      this.$content = $('.' + this.CssClasses.LAYOUT_CONTENT);
      this.$footer = $('.' + this.CssClasses.LAYOUT_FOOTER);
      this.$scrollableArea = $('.' + this.CssClasses.CUSTOM_SCROLLBAR);
      this.$sidenav = $('.' + this.CssClasses.SIDENAV);
      this.$sidenavBtn = $('.' + this.CssClasses.SIDENAV_BTN);
      this.$searchForm = $('.' + this.CssClasses.SEARCH_FORM);
      this.$searchFormBtn = $('.' + this.CssClasses.SEARCH_FORM_BTN);
      this.$themePanel = $('.' + this.CssClasses.THEME_PANEL);
      this.$themePanelBtn = $('.' + this.CssClasses.THEME_PANEL_BTN);
      this.$themeSettings = this.$themePanel.find(':checkbox');

      var mediaQueryString = '(max-width: ' + this.Constants.MEDIA_QUERY_BREAKPOINT + ')';
      this.mediaQueryList = window.matchMedia(mediaQueryString);

      if (this.mediaQueryMatches()) {
        this.collapseSidenav();
      }

      this.addCustomScrollbarTo(this.$scrollableArea);

      this.initPlugins().bindEvents().syncThemeSettings();
    },
    bindEvents: function bindEvents() {
      this.$document.on('keydown.e.app', this.handleKeyboardEvent.bind(this));

      this.$sidenavBtn.on('click.e.app', this.handleSidenavToggle.bind(this));
      this.$sidenav.on('collapse-start.e.app', this.handleSidenavCollapseStart.bind(this)).on('expand-start.e.app', this.handleSidenavExpandStart.bind(this));

      this.$sidenav.on('collapse-end.e.app', this.handleSidebarStickyUpdate.bind(this)).on('expand-end.e.app', this.handleSidebarStickyUpdate.bind(this));

      this.$sidenav.on('shown.metisMenu.e.app', this.handleSidebarStickyUpdate.bind(this)).on('hidden.metisMenu.e.app', this.handleSidebarStickyUpdate.bind(this));

      this.$searchFormBtn.on('click.e.app', this.handleSearchFormToggle.bind(this));

      this.$themePanelBtn.on('click.e.app', this.handleThemePanelToggle.bind(this));
      this.$themeSettings.on('change.e.app', this.handleThemeSettingsChange.bind(this));

      this.mediaQueryList.addListener(this.handleMediaQueryChange.bind(this));

      return this;
    },
    handleKeyboardEvent: function handleKeyboardEvent(evt) {
      if (/input|textarea/i.test(evt.target.tagName)) return;

      switch (evt.keyCode) {
        case this.KeyCodes.S:
          this.toggleSearchForm();
          break;
        case this.KeyCodes.OPEN_SQUARE_BRACKET:
          this.toggleSidenav();
          break;
        case this.KeyCodes.CLOSE_SQUARE_BRACKET:
          this.toggleThemePanel();
          break;
      }
    },
    handleSidenavToggle: function handleSidenavToggle(evt) {
      evt.preventDefault();
      this.toggleSidenav();
    },
    handleSidenavCollapseStart: function handleSidenavCollapseStart(evt) {
      var $input = this.getThemeSettingsBy(this.CssClasses.LAYOUT_SIDEBAR_COLLAPSED);

      $input.prop('checked', true);
    },
    handleSidenavExpandStart: function handleSidenavExpandStart(evt) {
      var $input = this.getThemeSettingsBy(this.CssClasses.LAYOUT_SIDEBAR_COLLAPSED);

      $input.prop('checked', false);
    },
    handleSidebarStickyUpdate: function handleSidebarStickyUpdate(evt) {
      if (this.isSidebarSticky()) {
        this.updateStickySidebar();
      }
    },
    handleSearchFormToggle: function handleSearchFormToggle(evt) {
      evt.preventDefault();
      this.toggleSearchForm();
    },
    handleThemePanelToggle: function handleThemePanelToggle(evt) {
      evt.preventDefault();
      this.toggleThemePanel();
    },
    handleThemeSettingsChange: function handleThemeSettingsChange(evt) {
      var $input = $(evt.target);

      switch ($input.attr('name')) {
        case this.CssClasses.LAYOUT_HEADER_FIXED:
          this.setHeaderFixed($input.prop('checked'));
          break;
        case this.CssClasses.LAYOUT_SIDEBAR_FIXED:
          this.setSidebarFixed($input.prop('checked'));
          break;
        case this.CssClasses.LAYOUT_SIDEBAR_STICKY:
          this.setSidebarSticky($input.prop('checked'));
          break;
        case this.CssClasses.LAYOUT_SIDEBAR_COLLAPSED:
          this.$sidenavBtn.trigger('click');
          break;
        case this.CssClasses.LAYOUT_FOOTER_FIXED:
          this.setFooterFixed($input.prop('checked'));
          break;
      }
    },
    handleMediaQueryChange: function handleMediaQueryChange(evt) {
      this[this.mediaQueryMatches() ? 'collapseSidenav' : 'expandSidenav']();
    },
    collapseSidenav: function collapseSidenav() {
      var startEvent = $.Event('collapse-start');

      this.$layout.addClass(this.CssClasses.LAYOUT_SIDEBAR_COLLAPSED);
      this.$sidenav.trigger(startEvent).hide();

      this.$sidenav.addClass(this.CssClasses.SIDENAV_COLLAPSED);
      this.$sidenavBtn.addClass(this.CssClasses.COLLAPSED);

      if (this.transitionTimeoutId) {
        clearTimeout(this.transitionTimeoutId);
      }

      this.transitionTimeoutId = setTimeout(function () {
        this.$sidenav.fadeIn(this.Constants.TRANSITION_DURATION).trigger('collapse-end');
      }.bind(this), this.Constants.TRANSITION_DELAY);

      this.$sidenav.attr('aria-expanded', false);
      this.$sidenavBtn.attr('aria-expanded', false).attr('title', 'Expand sidenav ( [ )');
    },
    expandSidenav: function expandSidenav() {
      var startEvent = $.Event('expand-start');

      this.$layout.removeClass(this.CssClasses.LAYOUT_SIDEBAR_COLLAPSED);
      this.$sidenav.trigger(startEvent).hide();

      this.$sidenav.removeClass(this.CssClasses.SIDENAV_COLLAPSED);
      this.$sidenavBtn.removeClass(this.CssClasses.COLLAPSED);

      if (this.transitionTimeoutId) {
        clearTimeout(this.transitionTimeoutId);
      }

      this.transitionTimeoutId = setTimeout(function () {
        this.$sidenav.fadeIn(this.Constants.TRANSITION_DURATION).trigger('expand-end');
      }.bind(this), this.Constants.TRANSITION_DELAY);

      this.$sidenav.attr('aria-expanded', true);
      this.$sidenavBtn.attr('aria-expanded', true).attr('title', 'Collapse sidenav ( [ )');
    },
    toggleSidenav: function toggleSidenav() {
      this[this.isSidenavCollapsed() ? 'expandSidenav' : 'collapseSidenav']();
    },
    isSidenavCollapsed: function isSidenavCollapsed() {
      return this.$sidenav.hasClass(this.CssClasses.SIDENAV_COLLAPSED);
    },
    toggleSearchForm: function toggleSearchForm() {
      this.$searchForm.toggleClass(this.CssClasses.SEARCH_FORM_COLLAPSED);
      this.$searchFormBtn.toggleClass(this.CssClasses.COLLAPSED);

      if (this.isSearchFormCollapsed()) {
        this.$searchForm.attr('aria-expanded', false);
        this.$searchFormBtn.attr('aria-expanded', false).attr('title', 'Expand search form ( S )');
      } else {
        this.$searchForm.attr('aria-expanded', true);
        this.$searchFormBtn.attr('aria-expanded', true).attr('title', 'Collapse search form ( S )');
      }
    },
    isSearchFormCollapsed: function isSearchFormCollapsed() {
      return this.$searchForm.hasClass(this.CssClasses.SEARCH_FORM_COLLAPSED);
    },
    toggleThemePanel: function toggleThemePanel() {
      this.$themePanel.toggleClass(this.CssClasses.THEME_PANEL_COLLAPSED);
      this.$themePanelBtn.toggleClass(this.CssClasses.COLLAPSED);

      if (this.isThemePanelCollapsed()) {
        this.$themePanel.attr('aria-expanded', false);
        this.$themePanelBtn.attr('aria-expanded', false).attr('title', 'Expand theme panel ( ] )');
      } else {
        this.$themePanel.attr('aria-expanded', true);
        this.$themePanelBtn.attr('aria-expanded', true).attr('title', 'Collapse theme panel ( ] )');

        this.showShareMessage();
      }
    },
    isThemePanelCollapsed: function isThemePanelCollapsed() {
      return this.$themePanel.hasClass(this.CssClasses.THEME_PANEL_COLLAPSED);
    },
    syncThemeSettings: function syncThemeSettings() {
      var settings = {};

      this.$themeSettings.each(function (idx, input) {
        var $input = $(input),
            name = $input.attr('name');

        if ($input.data('sync')) {
          settings[name] = this.$layout.hasClass(name);
        }
      }.bind(this));

      this.changeThemeSettings(settings);

      return this;
    },
    changeThemeSettings: function changeThemeSettings(settings) {
      $.each(settings, function (name, state) {
        var $input = this.getThemeSettingsBy(name);
        $input.prop('checked', state).trigger('change');
      }.bind(this));

      return this;
    },
    getThemeSettingsBy: function getThemeSettingsBy(name) {
      return this.$themeSettings.filter("[name='" + name + "']");
    },
    isShareMessageShown: function isShareMessageShown() {
      return !!Cookies.get(this.Constants.SHARE_MESSAGE_KEY);
    },
    setShareMessageShown: function setShareMessageShown() {
      var key,
          val,
          attr = {};

      key = this.Constants.SHARE_MESSAGE_KEY;
      val = this.Constants.SHARE_MESSAGE_STATUS;
      attr.expires = this.Constants.SHARE_MESSAGE_EXPIRES;

      Cookies.set(key, val, attr);
    },
    showShareMessage: function showShareMessage() {
      var title, message, options;

      if (!this.isShareMessageShown()) {
        options = this.getShareMessageOptions();
        message = 'If you like Elephant, please share it with your friends ' + 'and followers, this way you will help the elephant grow.';

        toastr.info(message, title, options);
        this.setShareMessageShown();
      }
    },
    isHeaderStatic: function isHeaderStatic() {
      return !this.$layout.hasClass(this.CssClasses.LAYOUT_HEADER_FIXED);
    },
    setHeaderFixed: function setHeaderFixed(state) {
      var settings = {};

      this.$layout.toggleClass(this.CssClasses.LAYOUT_HEADER_FIXED, state);

      if (this.isHeaderStatic() && this.isSidebarFixed()) {
        settings[this.CssClasses.LAYOUT_SIDEBAR_FIXED] = state;
      }

      if (this.isHeaderStatic() && this.isSidebarSticky()) {
        settings[this.CssClasses.LAYOUT_SIDEBAR_STICKY] = state;
      }
      this.changeThemeSettings(settings);
    },
    isSidebarFixed: function isSidebarFixed() {
      return this.$layout.hasClass(this.CssClasses.LAYOUT_SIDEBAR_FIXED);
    },
    setSidebarFixed: function setSidebarFixed(state) {
      var settings = {},
          $sidebar = this.getSidebarScrollableArea();

      this.$layout.toggleClass(this.CssClasses.LAYOUT_SIDEBAR_FIXED, state);

      if (!this.isSidebarFixed()) {
        return this.removeCustomScrollbarFrom($sidebar);
      }

      if (this.isHeaderStatic()) {
        settings[this.CssClasses.LAYOUT_HEADER_FIXED] = state;
      }

      if (this.isSidebarSticky()) {
        settings[this.CssClasses.LAYOUT_SIDEBAR_STICKY] = !state;
      }
      this.changeThemeSettings(settings).addCustomScrollbarTo($sidebar);
    },
    isSidebarSticky: function isSidebarSticky() {
      return this.$layout.hasClass(this.CssClasses.LAYOUT_SIDEBAR_STICKY);
    },
    setSidebarSticky: function setSidebarSticky(state) {
      var settings = {};

      this.$layout.toggleClass(this.CssClasses.LAYOUT_SIDEBAR_STICKY, state);

      if (!this.isSidebarSticky()) {
        return this.destroyStickySidebar();
      }

      if (this.isHeaderStatic()) {
        settings[this.CssClasses.LAYOUT_HEADER_FIXED] = state;
      }

      if (this.isSidebarFixed()) {
        settings[this.CssClasses.LAYOUT_SIDEBAR_FIXED] = !state;
      }
      this.changeThemeSettings(settings).createStickySidebar();
    },
    setFooterFixed: function setFooterFixed(state) {
      this.$layout.toggleClass(this.CssClasses.LAYOUT_FOOTER_FIXED, state);
    },
    addCustomScrollbarTo: function addCustomScrollbarTo($el) {
      var options = this.getCustomScrollbarOptions();
      $el.slimScroll(options);
    },
    removeCustomScrollbarFrom: function removeCustomScrollbarFrom($el) {
      var options = this.getCustomScrollbarOptions();

      options.destroy = true;

      $el.slimScroll(options).off().removeAttr('style');
    },
    createStickySidebar: function createStickySidebar() {
      var $target = this.getSidebarScrollableArea(),
          options = this.getStickyOptions();

      options.stickTo = this.$content;

      $target.hcSticky(options).hcSticky('reinit');
    },
    updateStickySidebar: function updateStickySidebar() {
      var $target = this.getSidebarScrollableArea();
      $target.hcSticky('reinit');
    },
    destroyStickySidebar: function destroyStickySidebar() {
      var $target = this.getSidebarScrollableArea();

      $target.data('hcSticky') && $target.hcSticky('destroy').off().removeAttr('style');
    },
    mediaQueryMatches: function mediaQueryMatches() {
      return this.mediaQueryList.matches;
    },
    getSidebarScrollableArea: function getSidebarScrollableArea() {
      return this.$sidebar.find('.' + this.CssClasses.CUSTOM_SCROLLBAR);
    },
    getCreateOptions: function getCreateOptions(prefix, callback) {
      var regex = new RegExp('^' + prefix + '(_)?', 'i'),
          options = {};

      $.each(this, function (prop, obj) {
        if (!$.isPlainObject(obj)) return;

        $.each(obj, function (key, val) {
          if (regex.test(key)) {
            key = key.replace(regex, '').replace(/_/g, '-');
            key = $.camelCase(key.toLowerCase());

            callback && callback(options, prop, key, val) || (options[key] = val);
          }
        });
      });

      return options;
    },
    getShareMessageOptions: function getShareMessageOptions() {
      return this.getCreateOptions('SHARE_MESSAGE');
    },
    getCustomScrollbarOptions: function getCustomScrollbarOptions() {
      var callback = function callback(options, prop, key, val) {
        key = prop === 'CssClasses' ? key + 'Class' : key;
        return options[key] = val;
      };

      return this.getCreateOptions('custom_scrollbar', callback);
    },
    getSelect2Options: function getSelect2Options() {
      return this.getCreateOptions('select2');
    },
    getSidenavOptions: function getSidenavOptions() {
      var callback = function callback(options, prop, key, val) {
        key = prop === 'CssClasses' ? key + 'Class' : key;
        return options[key] = val;
      };

      return this.getCreateOptions('sidenav', callback);
    },
    getStickyOptions: function getStickyOptions() {
      var callback = function callback(options, prop, key, val) {
        key = prop === 'CssClasses' ? key + 'ClassName' : key;
        return options[key] = val;
      };

      return this.getCreateOptions('sticky', callback);
    },
    initPlugins: function initPlugins() {
      this.matchHeight();
      this.metisMenu();
      this.select2();
      this.tooltip();
      this.vectorMap();

      return this;
    },
    matchHeight: function matchHeight() {
      $('[data-toggle="match-height"]').matchHeight();
    },
    metisMenu: function metisMenu() {
      var options = this.getSidenavOptions();
      this.$sidenav.metisMenu(options);
    },
    select2: function select2() {
      var Select2 = $.fn.select2,
          options = this.getSelect2Options();

      $.each(options, function (key, value) {
        Select2.defaults.set(key, value);
      });
    },
    tooltip: function tooltip() {
      $('[data-toggle="tooltip"]').tooltip();
    },
    vectorMap: function vectorMap() {
      $('[data-toggle="vector-map"]').each(function () {
        var $map = $(this),
            options = $map.data();

        $map.vectorMap(options);
      });
    }
  };

  App.init();
})(jQuery);
