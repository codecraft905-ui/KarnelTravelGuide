/* Karnel Travel Guide - site behaviour (no libraries). Everything here is progressive enhancement:
   the pages stay fully usable if this file does not load. */
(function () {
  'use strict';
  var root = document.documentElement;
  var reduce = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  /* Floating navigation: more solid once the page is scrolled */
  var nav = document.querySelector('header.nav');
  if (nav) {
    var onScroll = function () { nav.classList.toggle('scrolled', window.scrollY > 40); };
    onScroll(); window.addEventListener('scroll', onScroll, { passive: true });
    var toggle = document.getElementById('nt');
    if (toggle) {
      // close the mobile menu after choosing a link, or with Escape
      nav.addEventListener('click', function (e) { if (toggle.checked && e.target.closest && e.target.closest('nav a')) toggle.checked = false; });
      document.addEventListener('keydown', function (e) { if (e.key === 'Escape' && toggle.checked) { toggle.checked = false; var b = nav.querySelector('.burger'); if (b) b.focus(); } });
    }
  }

  /* Broken photo -> palette-matched placeholder instead of a broken-image icon */
  document.addEventListener('error', function (e) {
    var i = e.target; if (!i || i.tagName !== 'IMG' || i.dataset.g) return; i.dataset.g = 1;
    i.src = 'data:image/svg+xml,' + encodeURIComponent('<svg xmlns="http://www.w3.org/2000/svg" width="600" height="380"><defs><linearGradient id="g" x1="0" y1="0" x2="1" y2="1"><stop offset="0" stop-color="#2a2b32"/><stop offset="1" stop-color="#0f1013"/></linearGradient></defs><rect width="100%" height="100%" fill="url(#g)"/><rect x="40%" y="42%" width="20%" height="2" fill="#d4b87a" opacity=".6"/></svg>');
  }, true);

  /* Scroll reveals (staggered per group). Elements are only hidden by CSS while the "js" class is on <html>. */
  var targets = [].slice.call(document.querySelectorAll('.site main .sec, .site main .sechead, .site main .grid > *, .site main .pkgsum, .site main .two > *, .site main .faq details, .site main .gallery img, .site main .tsl, .site main .formcard, .site main .listbox, .site main .filters, .discover, .stats > div, .band .sechead, .why .card'));
  targets.forEach(function (el) { el.classList.add('rv'); });
  root.classList.add('ready');
  if (reduce || !('IntersectionObserver' in window)) {
    targets.forEach(function (el) { el.classList.add('in'); });
  } else {
    var seen = new Map();
    var io = new IntersectionObserver(function (entries) {
      entries.forEach(function (en) {
        if (!en.isIntersecting) return;
        var p = en.target.parentNode, k = (seen.get(p) || 0); seen.set(p, k + 1);
        en.target.style.setProperty('--d', Math.min(k, 6));
        en.target.classList.add('in'); io.unobserve(en.target);
        // after the entrance finishes, hand the element back to its normal hover transitions
        (function (el) { setTimeout(function () { el.classList.remove('rv', 'in'); el.style.removeProperty('--d'); }, 1400 + Number(el.style.getPropertyValue('--d') || 0) * 70); })(en.target);
      });
    }, { rootMargin: '0px 0px -8% 0px', threshold: 0.08 });
    targets.forEach(function (el) { io.observe(el); });
  }

  /* Home hero: slideshow with parallax, pause control, keyboard and swipe */
  var hero = document.getElementById('hs');
  if (hero) {
    var bgs = hero.querySelectorAll('.hs-bg'), txts = hero.querySelectorAll('.hs-txt'),
        cards = hero.querySelectorAll('.hs-card'), dots = hero.querySelectorAll('.hs-dot'),
        n = bgs.length, idx = 0, timer = null, paused = reduce, hovering = false;
    var cur = document.getElementById('hsCur'), live = hero.querySelector('.hs-txtwrap'), pbtn = document.getElementById('hsPause');
    var pad = function (x) { return (x < 10 ? '0' : '') + x; };
    var render = function () {
      for (var k = 0; k < n; k++) {
        bgs[k].classList.toggle('on', k === idx); txts[k].classList.toggle('on', k === idx);
        if (dots[k]) { dots[k].classList.toggle('on', k === idx); dots[k].setAttribute('aria-current', k === idx ? 'true' : 'false'); }
        var c = cards[k], p = (k - idx + n) % n, old = parseInt(c.getAttribute('data-p'), 10);
        var jump = Math.abs(p - old) > 1;
        if (jump) c.style.transition = 'none';
        c.setAttribute('data-p', p); c.style.setProperty('--p', p); c.tabIndex = p === 0 ? -1 : 0; c.setAttribute('aria-hidden', p === 0 ? 'true' : 'false');
        if (jump) { void c.offsetWidth; c.style.transition = ''; }
      }
      if (cur) cur.textContent = pad(idx + 1);
    };
    var stop = function () { clearInterval(timer); timer = null; };
    var play = function () { stop(); if (!paused && !hovering && n > 1) timer = setInterval(function () { go(idx + 1, true); }, 3000); };
    var go = function (i, auto) { idx = (i + n) % n; render(); if (!auto) play(); };
    var setPaused = function (v) {
      paused = v; if (pbtn) { pbtn.setAttribute('aria-pressed', String(v)); pbtn.setAttribute('aria-label', v ? 'Play slideshow' : 'Pause slideshow'); pbtn.classList.toggle('is-paused', v); }
      if (live) live.setAttribute('aria-live', v ? 'polite' : 'off'); play();
    };
    var nx = document.getElementById('hsNext'), pv = document.getElementById('hsPrev');
    if (nx) nx.addEventListener('click', function () { go(idx + 1); });
    if (pv) pv.addEventListener('click', function () { go(idx - 1); });
    if (pbtn) pbtn.addEventListener('click', function () { setPaused(!paused); });
    dots.forEach(function (d, i) { d.addEventListener('click', function () { go(i); }); });
    cards.forEach(function (c, i) { c.addEventListener('click', function () { if (c.getAttribute('data-p') !== '0') go(i); }); });
    hero.addEventListener('mouseenter', function () { hovering = true; stop(); });
    hero.addEventListener('mouseleave', function () { hovering = false; play(); });
    hero.addEventListener('focusin', function () { stop(); });
    hero.addEventListener('focusout', function () { play(); });
    var sx = 0;
    hero.addEventListener('touchstart', function (e) { sx = e.touches[0].clientX; }, { passive: true });
    hero.addEventListener('touchend', function (e) { var dx = e.changedTouches[0].clientX - sx; if (Math.abs(dx) > 50) go(idx + (dx < 0 ? 1 : -1)); }, { passive: true });
    setPaused(paused); render();

    // subtle parallax: the photo layer drifts slower than the page (disabled for reduced motion)
    var layer = hero.querySelector('.hs-parallax');
    if (layer && !reduce) {
      var ticking = false;
      var move = function () { var y = Math.min(window.scrollY, hero.offsetHeight); layer.style.transform = 'translate3d(0,' + (y * 0.22).toFixed(1) + 'px,0)'; ticking = false; };
      window.addEventListener('scroll', function () { if (!ticking) { ticking = true; requestAnimationFrame(move); } }, { passive: true });
    }
  }

  /* Home testimonials slider */
  var box = document.getElementById('tsl');
  if (box) {
    var slides = box.querySelectorAll('.tsl-slide'), tdots = box.querySelectorAll('.tsl-dot'), ti = 0, tt;
    var show = function (k) {
      ti = (k + slides.length) % slides.length;
      slides.forEach(function (s, j) { s.classList.toggle('on', j === ti); s.setAttribute('aria-hidden', j === ti ? 'false' : 'true'); });
      tdots.forEach(function (d, j) { d.classList.toggle('on', j === ti); d.setAttribute('aria-current', j === ti ? 'true' : 'false'); });
    };
    var tstop = function () { clearInterval(tt); };
    var tplay = function () { tstop(); if (!reduce) tt = setInterval(function () { show(ti + 1); }, 7000); };
    var tp = box.querySelector('.prev'), tn = box.querySelector('.next');
    if (tp) tp.addEventListener('click', function () { show(ti - 1); tplay(); });
    if (tn) tn.addEventListener('click', function () { show(ti + 1); tplay(); });
    tdots.forEach(function (d, j) { d.addEventListener('click', function () { show(j); tplay(); }); });
    box.addEventListener('mouseenter', tstop); box.addEventListener('mouseleave', tplay); box.addEventListener('focusin', tstop);
    show(0); tplay();
  }

  /* Booking form: show what the chosen package includes (was inline in the view) */
  var pkg = document.getElementById('pkg');
  if (pkg) {
    var sync = function () { [].forEach.call(document.querySelectorAll('.pkgbox'), function (b) { b.hidden = b.getAttribute('data-pkg') !== pkg.value; }); };
    pkg.addEventListener('change', sync); sync();
  }

  /* Contact form: rating only for feedback (was inline in the view) */
  var type = document.getElementById('msgType'), rating = document.getElementById('fbRating');
  if (type && rating) {
    var sy = function () { rating.hidden = type.value !== 'Feedback'; };
    type.addEventListener('change', sy); sy();
  }
})();
