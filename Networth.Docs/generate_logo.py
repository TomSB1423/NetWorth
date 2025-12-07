import math

def generate_logo():
    width = 200
    height = 200

    # Networth Purple Palette
    dark = "#5b1a96"
    light = "#a866e2"
    white = "#ffffff"

    # Shading colors
    shadow = "#3a0e66" # Darker purple for shadows

    # Common definitions for gradients and clipping
    defs = f'''
    <defs>
      <linearGradient id="bgGrad" x1="0%" y1="0%" x2="100%" y2="100%">
        <stop offset="0%" style="stop-color:{light};stop-opacity:1" />
        <stop offset="100%" style="stop-color:{dark};stop-opacity:1" />
      </linearGradient>
      <clipPath id="circleClip">
        <circle cx="100" cy="100" r="90" />
      </clipPath>
    </defs>
    '''

    # Option 1: Faceted/Low-Poly Mountain (Modern, Tech)
    # Features: Central peak, split lighting (light/shadow faces), back range, clipped to circle
    svg_v1 = f'''<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 {width} {height}">
  {defs}

  <!-- Background -->
  <circle cx="100" cy="100" r="90" fill="url(#bgGrad)" />

  <!-- Content Clipped to Circle -->
  <g clip-path="url(#circleClip)">

    <!-- Sun/Moon decoration -->
    <circle cx="150" cy="50" r="15" fill="{white}" opacity="0.1" />

    <!-- Back Range (Faded) -->
    <path d="M-20 150 L40 90 L80 150 L120 100 L160 160 L200 120 L240 200 L-20 200 Z" fill="{white}" opacity="0.15" />

    <!-- Main Mountain Group -->
    <g transform="translate(0, 10)">
        <!-- Left Face (Lit) -->
        <path d="M100 40 L40 160 L100 160 Z" fill="{white}" opacity="0.95" />
        <!-- Right Face (Shadowed) -->
        <path d="M100 40 L160 160 L100 160 Z" fill="{shadow}" opacity="0.3" />

        <!-- Snow Cap Left -->
        <path d="M100 40 L85 70 L92 65 L100 80 Z" fill="{white}" />
        <!-- Snow Cap Right -->
        <path d="M100 40 L115 70 L108 65 L100 80 Z" fill="#e0e0e0" />
    </g>

    <!-- Foreground Hills -->
    <path d="M-20 170 Q 50 150 100 170 T 220 170 V 210 H -20 Z" fill="{white}" opacity="0.1" />
  </g>
</svg>'''

    # Option 2: Layered Landscape (Scenic, Depth)
    svg_v2 = f'''<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 {width} {height}">
  {defs}
  <circle cx="100" cy="100" r="90" fill="url(#bgGrad)" />
  <g clip-path="url(#circleClip)">
    <!-- Sky Elements -->
    <circle cx="100" cy="100" r="70" fill="{white}" opacity="0.05" />
    <circle cx="100" cy="100" r="50" fill="{white}" opacity="0.05" />

    <!-- Layer 1: Furthest -->
    <path d="M0 130 L50 70 L100 120 L150 60 L200 130 V 200 H 0 Z" fill="{white}" opacity="0.2" />

    <!-- Layer 2: Mid -->
    <path d="M-10 200 L60 100 L130 200 Z" fill="{white}" opacity="0.5" />
    <path d="M210 200 L140 100 L70 200 Z" fill="{white}" opacity="0.4" />

    <!-- Layer 3: Center Peak with Shadow -->
    <path d="M100 50 L160 180 H 40 Z" fill="{white}" opacity="0.9" />
    <path d="M100 50 L160 180 L100 180 Z" fill="{shadow}" opacity="0.2" />

    <!-- Snow Cap -->
    <path d="M100 50 L115 85 L100 75 L85 85 Z" fill="{white}" />
  </g>
</svg>'''

    # Write files
    with open('static/img/networth-icon-v1.svg', 'w') as f:
        f.write(svg_v1)
    with open('static/img/networth-icon-v2.svg', 'w') as f:
        f.write(svg_v2)

    # Set v1 as default for now
    with open('static/img/networth-icon.svg', 'w') as f:
        f.write(svg_v1)

    print("Generated static/img/networth-icon.svg (and v1/v2 variants)")

if __name__ == "__main__":
    generate_logo()
