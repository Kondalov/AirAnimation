# MapLibre & Mapbox GL JS Guidelines

## Custom HTML Marker Centering (The 0x0 Wrapper Pattern)
When creating custom HTML DOM markers via `new maplibregl.Marker({ element: el })`, **never rely on MapLibre's default anchor centering or CSS `transform: translate(-50%, -50%)` on the root element.**
Because elements created in JavaScript yield `0x0` from `getBoundingClientRect()` before DOM insertion, MapLibre will incorrectly anchor the top-left corner to the map coordinate. Additionally, MapLibre overrides the root element's `transform` property, breaking CSS centering.

**Always use the 0x0 Wrapper Pattern:**
1. Create a root `wrapper` div with `position: relative`, `width: 0px`, and `height: 0px`. This guarantees MapLibre's internal offset calculation is exactly `0x0`.
2. Create an `inner` div containing your SVG or HTML.
3. Center the `inner` div using absolute positioning: `position: absolute; left: -{width/2}px; top: -{height/2}px;`.
4. Append `inner` to `wrapper`, and pass `wrapper` to the Marker constructor.

**Example:**
```javascript
const sz = 84;
const wrapper = document.createElement('div');
wrapper.style.position = 'relative';
wrapper.style.width = '0px';
wrapper.style.height = '0px';

const inner = document.createElement('div');
inner.style.position = 'absolute';
inner.style.left = `-${sz / 2}px`;
inner.style.top = `-${sz / 2}px`;
inner.style.width = `${sz}px`;
inner.style.height = `${sz}px`;
inner.innerHTML = '<svg>...</svg>';

wrapper.appendChild(inner);
new maplibregl.Marker({ element: wrapper }).setLngLat([lon, lat]).addTo(map);
```
