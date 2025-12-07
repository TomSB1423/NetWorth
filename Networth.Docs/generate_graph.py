# /// script
# requires-python = ">=3.12"
# dependencies = [
#     "matplotlib",
#     "numpy",
# ]
# ///

import matplotlib.pyplot as plt
import numpy as np
from pathlib import Path

# Get the directory where the script is located
script_dir = Path(__file__).parent
# Ensure the output directory exists relative to the script
output_dir = script_dir / 'static' / 'img'
output_dir.mkdir(parents=True, exist_ok=True)

# Parameters
monthly_contribution = 300
r = 0.07  # Interest Rate
years = 25
t = np.linspace(0, years, 100)

# Calculations
# Total Contributed (Linear)
# Contribution per year = monthly_contribution * 12
total_contributed = monthly_contribution * 12 * t

# Simple Interest Total (Approximate)
# Interest on each contribution accumulates linearly.
# Formula for sum of simple interest on regular contributions: PMT * 12 * r * t^2 / 2
simple_interest_total = total_contributed + (monthly_contribution * 12 * r * (t**2) / 2)

# Compound Interest Total (Future Value of Annuity)
# FV = PMT * ((1 + r/n)^(nt) - 1) / (r/n)
n = 12
# Avoid division by zero at t=0
compound_interest_total = np.zeros_like(t)
mask = t > 0
compound_interest_total[mask] = monthly_contribution * ((1 + r/n)**(n*t[mask]) - 1) / (r/n)

# Data for stacking
# Layer 1: Total Contributed
y1 = total_contributed
# Layer 2: Simple Interest Component (Total Simple - Contributed)
y2 = simple_interest_total - total_contributed
# Layer 3: Compound Interest Component (Total Compound - Total Simple)
y3 = compound_interest_total - simple_interest_total

# Plotting
fig, ax = plt.subplots(figsize=(10, 6))

# Stackplot with default colors
ax.stackplot(t, y1, y2, y3, 
             labels=['Total Contributed', 'Interest (No Compounding)', 'Compound Interest'],
             alpha=0.8)

# Styling
max_val = 250000
ax.set_xlim(0, years + 8)
ax.set_ylim(0, max_val)

# Remove top and right spines
ax.spines['top'].set_visible(False)
ax.spines['right'].set_visible(True)
ax.spines['right'].set_color('black')
ax.spines['left'].set_visible(False) # Clean look

# Title and Subtitle
ax.text(0, max_val * 1.08, 'Compound Interest Example', fontsize=20, fontweight='bold', color='black', ha='left')
ax.text(0, max_val * 1.02, '£300/month at 7% for 25 years', fontsize=14, color='gray', ha='left')

# Labels on the graph - Positioned to the right with arrows
target_x = years - 1
idx = int(len(t) * (target_x/years))
label_x = years + 1

# Principal Label
y_target = y1[idx]/2
ax.annotate('TOTAL\nCONTRIBUTED', 
            xy=(target_x, y_target), 
            xytext=(label_x, y_target),
            arrowprops=dict(arrowstyle='->', color='black'),
            color='black', fontsize=10, fontweight='bold', ha='left', va='center')

# Simple Interest Label
y_target = y1[idx] + y2[idx]/2
ax.annotate('INTEREST\n(NO COMPOUNDING)', 
            xy=(target_x, y_target), 
            xytext=(label_x, y_target),
            arrowprops=dict(arrowstyle='->', color='black'),
            color='black', fontsize=10, fontweight='bold', ha='left', va='center')

# Compound Interest Label
y_target = y1[idx] + y2[idx] + y3[idx]/2
ax.annotate('COMPOUND\nINTEREST', 
            xy=(target_x, y_target), 
            xytext=(label_x, y_target),
            arrowprops=dict(arrowstyle='->', color='black'),
            color='black', fontsize=12, fontweight='bold', ha='left', va='center')

# X-axis ticks
ax.set_xticks(np.arange(0, years + 1, 5))
ax.set_xticklabels(np.arange(0, years + 1, 5), fontsize=12)
ax.set_xlabel("Years", fontsize=12)

# Y-axis ticks (Right side)
ax.yaxis.tick_right()
ax.set_yticks(np.arange(0, max_val + 1, 50000))
ax.set_yticklabels([f'£{int(x):,}' for x in np.arange(0, max_val + 1, 50000)], fontsize=12)

# Save
output_path = output_dir / 'compound-interest-graph.png'
plt.tight_layout()
plt.savefig(output_path, dpi=300, bbox_inches='tight')

print(f"Graph saved to {output_path}")
