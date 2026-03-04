// =============================================
// 1. Scroll-based header show/hide
// =============================================
(function () {
  const header = document.querySelector('.site-header');
  if (!header) return;

  let lastScrollY = window.scrollY;
  let ticking = false;

  function onScroll() {
    const currentY = window.scrollY;

    if (currentY <= 0) {
      header.classList.remove('header-hidden');
    } else if (currentY > lastScrollY) {
      header.classList.add('header-hidden');
    } else {
      header.classList.remove('header-hidden');
    }

    lastScrollY = currentY;
    ticking = false;
  }

  window.addEventListener('scroll', function () {
    if (!ticking) {
      requestAnimationFrame(onScroll);
      ticking = true;
    }
  }, { passive: true });
})();

// =============================================
// 2. Fancy UI toggle (default OFF)
// =============================================
(function () {
  const STORAGE_KEY = 'fancyUiEnabled';

  // Create toggle widget
  const wrapper = document.createElement('div');
  wrapper.className = 'fancy-toggle';
  wrapper.innerHTML = `
    <label>
      <input type="checkbox" class="fancy-toggle__input" id="fancyToggle">
      <span class="fancy-toggle__track"></span>
      <span class="fancy-toggle__label">Fancy UI</span>
    </label>
  `;
  document.body.appendChild(wrapper);

  const checkbox = document.getElementById('fancyToggle');

  function setFancy(enabled) {
    if (enabled) {
      document.body.classList.add('fancy-ui');
    } else {
      document.body.classList.remove('fancy-ui');
    }
    checkbox.checked = enabled;
    try { localStorage.setItem(STORAGE_KEY, enabled ? '1' : '0'); } catch (e) { }
  }

  // Read stored preference — default OFF
  let stored = null;
  try { stored = localStorage.getItem(STORAGE_KEY); } catch (e) { }
  setFancy(stored === '1');

  checkbox.addEventListener('change', function () {
    setFancy(this.checked);
  });
})();

// =============================================
// 3. Dynamic glow colour from background
// =============================================
(function () {
  // Sample the dominant colour from the background image area behind the header
  // and set --glow-color on the body so the glow matches.
  function sampleBackgroundColor() {
    // Create a small off-screen canvas
    const canvas = document.createElement('canvas');
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    // We'll try to find the background image from body::before
    const bgStyle = getComputedStyle(document.body, '::before');
    const bgImage = bgStyle.backgroundImage;

    // Extract URL from background-image
    const urlMatch = bgImage && bgImage.match(/url\(["']?(.+?)["']?\)/);
    if (!urlMatch) {
      // Fallback: use default Discord-ish blue
      return;
    }

    const img = new Image();
    img.crossOrigin = 'anonymous';
    img.onload = function () {
      // Sample from the top-center strip (where the header sits)
      const sampleW = 200;
      const sampleH = 60;
      canvas.width = sampleW;
      canvas.height = sampleH;

      // Draw the top-center of the image
      const sx = (img.width - sampleW) / 2;
      const sy = 0;
      ctx.drawImage(img, sx, sy, sampleW, sampleH, 0, 0, sampleW, sampleH);

      const data = ctx.getImageData(0, 0, sampleW, sampleH).data;
      let r = 0, g = 0, b = 0, count = 0;

      // Average every 8th pixel for speed
      for (let i = 0; i < data.length; i += 4 * 8) {
        r += data[i];
        g += data[i + 1];
        b += data[i + 2];
        count++;
      }

      if (count > 0) {
        r = Math.round(r / count);
        g = Math.round(g / count);
        b = Math.round(b / count);

        // Boost saturation a bit so the glow is more visible
        const max = Math.max(r, g, b);
        const min = Math.min(r, g, b);
        const mid = (max + min) / 2;

        // Push colours away from grey towards their direction
        const boost = 1.6;
        r = Math.min(255, Math.round(mid + (r - mid) * boost));
        g = Math.min(255, Math.round(mid + (g - mid) * boost));
        b = Math.min(255, Math.round(mid + (b - mid) * boost));

        // Ensure minimum brightness for the glow
        const brightness = (r + g + b) / 3;
        if (brightness < 80) {
          const factor = 80 / Math.max(brightness, 1);
          r = Math.min(255, Math.round(r * factor));
          g = Math.min(255, Math.round(g * factor));
          b = Math.min(255, Math.round(b * factor));
        }

        document.body.style.setProperty('--glow-color', `${r}, ${g}, ${b}`);
      }
    };

    img.onerror = function () {
      // Silently fall back to CSS default
    };

    img.src = urlMatch[1];
  }

  // Run after a tiny delay so layout + background are ready
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', function () {
      setTimeout(sampleBackgroundColor, 200);
    });
  } else {
    setTimeout(sampleBackgroundColor, 200);
  }
})();
