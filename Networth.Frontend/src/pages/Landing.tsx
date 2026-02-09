import { useState, useMemo, useCallback, useRef, memo, useEffect } from "react";
import {
    TrendingUp,
    Eye,
    Target,
    Home,
    Baby,
    Briefcase,
    BarChart3,
    Pause,
    Play,
    ChevronRight,
    Menu,
    X,
    Sparkles,
} from "lucide-react";
import { Switch } from "../components/ui/switch";
import {
    ResponsiveContainer,
    ComposedChart,
    Area,
    Line,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    ReferenceLine,
} from "recharts";

interface LandingPageProps {
    onGetStarted: () => void;
    onSignIn: () => void;
}

// Scenario configuration
interface Scenario {
    id: string;
    label: string;
    icon: typeof Home;
    color: string;
    enabled: boolean;
    age: number;
    minAge: number;
    maxAge: number;
    description: string;
}

interface ProjectionPoint {
    age: number;
    cash: number;
    investments: number;
    pension: number;
    property: number;
    liabilities: number;
    netWorth: number;
    events: string[];
}

// Feature tabs configuration for slideshow
const FEATURE_TABS = [
    {
        id: "track",
        label: "Track",
        description:
            "See your complete financial picture in real-time. All accounts, all balances, one dashboard.",
        image: "/dashboard-preview.png",
    },
    {
        id: "connect",
        label: "Connect",
        description:
            "Link your bank accounts securely with read-only access. Automatic sync keeps everything up to date.",
        image: "/accounts-preview.png",
    },
    {
        id: "analyze",
        label: "Analyze",
        description:
            "Understand your spending patterns and track your wealth trajectory over time.",
        image: "/transactions-preview.png",
    },
    {
        id: "grow",
        label: "Grow",
        description:
            "Set financial goals, track milestones, and watch your net worth grow.",
        image: "/dashboard-preview.png",
    },
] as const;

// Chart category configuration
const CATEGORY_CONFIG = {
    pension: { label: "Pension", color: "#8B5CF6" },
    investments: { label: "Investments", color: "#06B6D4" },
    property: { label: "Property", color: "#3B82F6" },
    cash: { label: "Cash", color: "#10B981" },
    liabilities: { label: "Liabilities", color: "#EF4444" },
    netWorth: { label: "Net Worth", color: "#F59E0B" },
};

// Bank logos for the scrolling carousel - using Google Favicon API for reliability
const BANK_LOGOS = [
    {
        name: "HSBC",
        logo: "https://www.google.com/s2/favicons?domain=hsbc.com&sz=128",
    },
    {
        name: "Barclays",
        logo: "https://www.google.com/s2/favicons?domain=barclays.co.uk&sz=128",
    },
    {
        name: "Lloyds",
        logo: "https://www.google.com/s2/favicons?domain=lloydsbank.com&sz=128",
    },
    {
        name: "NatWest",
        logo: "https://www.google.com/s2/favicons?domain=natwest.com&sz=128",
    },
    {
        name: "Santander",
        logo: "https://www.google.com/s2/favicons?domain=santander.co.uk&sz=128",
    },
    {
        name: "Nationwide",
        logo: "https://www.google.com/s2/favicons?domain=nationwide.co.uk&sz=128",
    },
    {
        name: "Monzo",
        logo: "https://www.google.com/s2/favicons?domain=monzo.com&sz=128",
    },
    {
        name: "Starling",
        logo: "https://www.google.com/s2/favicons?domain=starlingbank.com&sz=128",
    },
    {
        name: "Revolut",
        logo: "https://www.google.com/s2/favicons?domain=revolut.com&sz=128",
    },
    {
        name: "Chase",
        logo: "https://www.google.com/s2/favicons?domain=chase.com&sz=128",
    },
    {
        name: "Metro Bank",
        logo: "https://www.google.com/s2/favicons?domain=metrobankonline.co.uk&sz=128",
    },
    {
        name: "TSB",
        logo: "https://www.google.com/s2/favicons?domain=tsb.co.uk&sz=128",
    },
    {
        name: "Halifax",
        logo: "https://www.google.com/s2/favicons?domain=halifax.co.uk&sz=128",
    },
    {
        name: "First Direct",
        logo: "https://www.google.com/s2/favicons?domain=firstdirect.com&sz=128",
    },
    {
        name: "Co-op Bank",
        logo: "https://www.google.com/s2/favicons?domain=co-operativebank.co.uk&sz=128",
    },
    {
        name: "Virgin Money",
        logo: "https://www.google.com/s2/favicons?domain=virginmoney.com&sz=128",
    },
];

// Format currency for display
function formatCurrency(value: number): string {
    const absValue = Math.abs(value);
    if (absValue >= 1000000) {
        return `£${(value / 1000000).toFixed(1)}M`;
    }
    if (absValue >= 1000) {
        return `£${(value / 1000).toFixed(0)}k`;
    }
    return `£${value.toFixed(0)}`;
}

// Generate projection data based on scenarios
function generateProjection(scenarios: Scenario[]): ProjectionPoint[] {
    const startAge = 25;
    const endAge = 70;
    const data: ProjectionPoint[] = [];

    // Base assumptions (mock data)
    const annualSalary = 45000;
    const savingsRate = 0.15;
    const investmentReturn = 0.06;
    const pensionContribution = 0.08;
    const pensionReturn = 0.05;
    const propertyAppreciation = 0.03;
    const mortgageRate = 0.045;
    const mortgageTerm = 25;

    let cash = 8000;
    let investments = 15000;
    let pension = 12000;
    let property = 0;
    let mortgageBalance = 0;
    let otherLiabilities = 5000; // Student loan etc

    const buyHouse = scenarios.find((s) => s.id === "buyHouse");
    const haveKids = scenarios.find((s) => s.id === "haveKids");
    const careerBreak = scenarios.find((s) => s.id === "careerBreak");

    for (let age = startAge; age <= endAge; age++) {
        const events: string[] = [];
        let monthlySavings = (annualSalary * savingsRate) / 12;
        let monthlyPensionContrib = (annualSalary * pensionContribution) / 12;

        // Buy house scenario
        if (buyHouse?.enabled && age === buyHouse.age) {
            property = 350000;
            mortgageBalance = 280000;
            cash -= 70000; // Deposit
            if (cash < 0) {
                investments += cash;
                cash = 2000;
            }
            events.push("🏠 Buy house");
        }

        // Have kids scenario - ongoing impact
        if (
            haveKids?.enabled &&
            age >= haveKids.age &&
            age < haveKids.age + 18
        ) {
            const kidsImpact = 1200; // Monthly cost
            monthlySavings -= kidsImpact / 12;
            if (age === haveKids.age) {
                events.push("👶 Have children");
            }
        }

        // Career break scenario
        if (
            careerBreak?.enabled &&
            age >= careerBreak.age &&
            age < careerBreak.age + 1
        ) {
            monthlySavings = -2000; // Spending savings
            monthlyPensionContrib = 0;
            if (age === careerBreak.age) {
                events.push("✈️ Career break");
            }
        }

        // Apply annual changes
        cash += monthlySavings * 12;
        cash = Math.max(cash, 1000);

        investments *= 1 + investmentReturn;
        investments += Math.max(0, monthlySavings * 6);

        pension *= 1 + pensionReturn;
        pension += monthlyPensionContrib * 12;

        if (property > 0) {
            property *= 1 + propertyAppreciation;
            // Pay down mortgage
            const monthlyMortgage =
                (mortgageBalance * (mortgageRate / 12)) /
                (1 - Math.pow(1 + mortgageRate / 12, -mortgageTerm * 12));
            mortgageBalance = Math.max(
                0,
                mortgageBalance - monthlyMortgage * 3,
            );
        }

        // Reduce other liabilities over time
        otherLiabilities = Math.max(0, otherLiabilities - 1500);

        const totalLiabilities = mortgageBalance + otherLiabilities;
        const totalAssets = cash + investments + pension + property;

        data.push({
            age,
            cash: Math.round(cash),
            investments: Math.round(investments),
            pension: Math.round(pension),
            property: Math.round(property),
            liabilities: -Math.round(totalLiabilities),
            netWorth: Math.round(totalAssets - totalLiabilities),
            events,
        });
    }

    return data;
}

// Fixed Y-axis domain - defined outside component to prevent recreation
const Y_DOMAIN: [number, number] = [-500000, 2500000];

// Chart gradient definitions - extracted to avoid recreation
const CHART_GRADIENTS = (
    <defs>
        <linearGradient id="cashGradient" x1="0" y1="0" x2="0" y2="1">
            <stop
                offset="5%"
                stopColor={CATEGORY_CONFIG.cash.color}
                stopOpacity={0.8}
            />
            <stop
                offset="95%"
                stopColor={CATEGORY_CONFIG.cash.color}
                stopOpacity={0.3}
            />
        </linearGradient>
        <linearGradient id="investmentsGradient" x1="0" y1="0" x2="0" y2="1">
            <stop
                offset="5%"
                stopColor={CATEGORY_CONFIG.investments.color}
                stopOpacity={0.8}
            />
            <stop
                offset="95%"
                stopColor={CATEGORY_CONFIG.investments.color}
                stopOpacity={0.3}
            />
        </linearGradient>
        <linearGradient id="propertyGradient" x1="0" y1="0" x2="0" y2="1">
            <stop
                offset="5%"
                stopColor={CATEGORY_CONFIG.property.color}
                stopOpacity={0.8}
            />
            <stop
                offset="95%"
                stopColor={CATEGORY_CONFIG.property.color}
                stopOpacity={0.3}
            />
        </linearGradient>
        <linearGradient id="pensionGradient" x1="0" y1="0" x2="0" y2="1">
            <stop
                offset="5%"
                stopColor={CATEGORY_CONFIG.pension.color}
                stopOpacity={0.8}
            />
            <stop
                offset="95%"
                stopColor={CATEGORY_CONFIG.pension.color}
                stopOpacity={0.3}
            />
        </linearGradient>
        <linearGradient id="liabilitiesGradient" x1="0" y1="0" x2="0" y2="1">
            <stop
                offset="5%"
                stopColor={CATEGORY_CONFIG.liabilities.color}
                stopOpacity={0.6}
            />
            <stop
                offset="95%"
                stopColor={CATEGORY_CONFIG.liabilities.color}
                stopOpacity={0.2}
            />
        </linearGradient>
    </defs>
);

// Memoized scenario card component
const ScenarioCard = memo(function ScenarioCard({
    scenario,
    onToggle,
    onAgeChange,
}: {
    scenario: Scenario;
    onToggle: () => void;
    onAgeChange: (age: number) => void;
}) {
    const Icon = scenario.icon;
    const [localAge, setLocalAge] = useState(scenario.age);
    const [prevScenarioAge, setPrevScenarioAge] = useState(scenario.age);
    const debounceRef = useRef<ReturnType<typeof setTimeout> | undefined>(
        undefined,
    );

    // Sync local age with prop changes (e.g., reset) — React recommended pattern
    if (scenario.age !== prevScenarioAge) {
        setPrevScenarioAge(scenario.age);
        setLocalAge(scenario.age);
    }

    // Cleanup debounce on unmount
    useEffect(() => {
        return () => {
            if (debounceRef.current) clearTimeout(debounceRef.current);
        };
    }, []);

    const handleSliderChange = useCallback(
        (e: React.ChangeEvent<HTMLInputElement>) => {
            const newAge = parseInt(e.target.value);
            setLocalAge(newAge);

            if (debounceRef.current) clearTimeout(debounceRef.current);
            debounceRef.current = setTimeout(() => onAgeChange(newAge), 50);
        },
        [onAgeChange],
    );

    const sliderProgress =
        ((localAge - scenario.minAge) / (scenario.maxAge - scenario.minAge)) *
        100;

    return (
        <div
            className={`relative p-4 rounded-xl border transition-all duration-300 cursor-pointer ${
                scenario.enabled
                    ? "bg-slate-800/80 border-slate-600"
                    : "bg-slate-900/50 border-slate-800"
            }`}
            onClick={onToggle}
        >
            <div className="flex items-center justify-between mb-3">
                <div className="flex items-center gap-3">
                    <div
                        className="w-10 h-10 rounded-lg flex items-center justify-center"
                        style={{
                            backgroundColor: scenario.enabled
                                ? `${scenario.color}20`
                                : "rgb(30 41 59)",
                        }}
                    >
                        <Icon
                            size={20}
                            style={{
                                color: scenario.enabled
                                    ? scenario.color
                                    : "#64748b",
                            }}
                        />
                    </div>
                    <div>
                        <h4 className="font-semibold text-white text-sm">
                            {scenario.label}
                        </h4>
                        <p className="text-xs text-slate-500">
                            {scenario.description}
                        </p>
                    </div>
                </div>
                <Switch
                    checked={scenario.enabled}
                    onCheckedChange={onToggle}
                    onClick={(e) => e.stopPropagation()}
                    aria-label={`Toggle ${scenario.label}`}
                />
            </div>

            <div
                className={`space-y-2 transition-opacity duration-300 ${scenario.enabled ? "" : "opacity-40 pointer-events-none"}`}
                onClick={(e) => e.stopPropagation()}
            >
                <div className="flex justify-between text-xs">
                    <span className="text-slate-400">Age</span>
                    <span
                        className="font-medium"
                        style={{
                            color: scenario.enabled
                                ? scenario.color
                                : "#64748b",
                        }}
                    >
                        {localAge}
                    </span>
                </div>
                <input
                    type="range"
                    min={scenario.minAge}
                    max={scenario.maxAge}
                    value={localAge}
                    onChange={handleSliderChange}
                    disabled={!scenario.enabled}
                    className={`w-full h-1.5 rounded-lg appearance-none ${scenario.enabled ? "cursor-pointer" : "cursor-not-allowed"}`}
                    style={{
                        background: scenario.enabled
                            ? `linear-gradient(to right, ${scenario.color} 0%, ${scenario.color} ${sliderProgress}%, rgb(51 65 85) ${sliderProgress}%, rgb(51 65 85) 100%)`
                            : "rgb(51 65 85)",
                    }}
                />
                <div className="flex justify-between text-[10px] text-slate-600">
                    <span>{scenario.minAge}</span>
                    <span>{scenario.maxAge}</span>
                </div>
            </div>
        </div>
    );
});

// Interactive chart component for the landing page
const InteractiveProjectionChart = memo(function InteractiveProjectionChart({
    scenarios,
    onToggleScenario,
    onChangeAge,
}: {
    scenarios: Scenario[];
    onToggleScenario: (id: string) => void;
    onChangeAge: (id: string, age: number) => void;
}) {
    const data = useMemo(() => generateProjection(scenarios), [scenarios]);
    const [, setHoveredAge] = useState<number | null>(null);

    const handleMouseMove = useCallback((state: unknown) => {
        const chartState = state as {
            activePayload?: { payload?: ProjectionPoint }[];
        };
        const age = chartState?.activePayload?.[0]?.payload?.age;
        if (age !== undefined) setHoveredAge(age);
    }, []);

    const handleMouseLeave = useCallback(() => setHoveredAge(null), []);

    // Event markers for enabled scenarios
    const eventMarkers = useMemo(
        () =>
            scenarios
                .filter((s) => s.enabled)
                .map((s) => ({
                    id: s.id,
                    age: s.age,
                    label: s.label,
                    color: s.color,
                })),
        [scenarios],
    );

    // Calculate key metrics for display
    const metrics = useMemo(() => {
        const startPoint = data[0];
        const endPoint = data[data.length - 1];
        const peakNetWorth = Math.max(...data.map((d) => d.netWorth));
        const ageAtPeak =
            data.find((d) => d.netWorth === peakNetWorth)?.age ?? 70;
        return {
            startNetWorth: startPoint?.netWorth ?? 0,
            endNetWorth: endPoint?.netWorth ?? 0,
            peakNetWorth,
            ageAtPeak,
            totalGrowth:
                (endPoint?.netWorth ?? 0) - (startPoint?.netWorth ?? 0),
        };
    }, [data]);

    return (
        <div className="space-y-6">
            {/* Scenario Controls */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                {scenarios.map((scenario) => (
                    <ScenarioCard
                        key={scenario.id}
                        scenario={scenario}
                        onToggle={() => onToggleScenario(scenario.id)}
                        onAgeChange={(age) => onChangeAge(scenario.id, age)}
                    />
                ))}
            </div>

            {/* Key Metrics Summary */}
            <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
                <div className="bg-slate-800/50 border border-slate-700/50 rounded-lg p-3 text-center">
                    <div className="text-xs text-slate-400 mb-1">
                        Starting (Age 25)
                    </div>
                    <div className="text-lg font-bold text-white">
                        {formatCurrency(metrics.startNetWorth)}
                    </div>
                </div>
                <div className="bg-slate-800/50 border border-slate-700/50 rounded-lg p-3 text-center">
                    <div className="text-xs text-slate-400 mb-1">
                        Peak Wealth
                    </div>
                    <div className="text-lg font-bold text-emerald-400">
                        {formatCurrency(metrics.peakNetWorth)}
                    </div>
                    <div className="text-[10px] text-slate-500">
                        at age {metrics.ageAtPeak}
                    </div>
                </div>
                <div className="bg-slate-800/50 border border-slate-700/50 rounded-lg p-3 text-center">
                    <div className="text-xs text-slate-400 mb-1">
                        At Retirement (70)
                    </div>
                    <div className="text-lg font-bold text-white">
                        {formatCurrency(metrics.endNetWorth)}
                    </div>
                </div>
                <div className="bg-slate-800/50 border border-slate-700/50 rounded-lg p-3 text-center">
                    <div className="text-xs text-slate-400 mb-1">
                        Total Growth
                    </div>
                    <div className="text-lg font-bold text-emerald-400">
                        +{formatCurrency(metrics.totalGrowth)}
                    </div>
                </div>
            </div>

            {/* Chart */}
            <div className="bg-slate-900/80 border border-slate-800 rounded-xl p-4 md:p-6 overflow-hidden">
                <ResponsiveContainer width="100%" height={350}>
                    <ComposedChart
                        data={data}
                        margin={{ top: 20, right: 20, left: 20, bottom: 20 }}
                        onMouseMove={handleMouseMove}
                        onMouseLeave={handleMouseLeave}
                    >
                        {CHART_GRADIENTS}
                        <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
                        <XAxis
                            dataKey="age"
                            tick={{ fontSize: 11, fill: "#94a3b8" }}
                            tickLine={false}
                            axisLine={{ stroke: "#334155" }}
                            label={{
                                value: "Age",
                                position: "bottom",
                                offset: 0,
                                fontSize: 11,
                                fill: "#94a3b8",
                            }}
                        />
                        <YAxis
                            domain={Y_DOMAIN}
                            tickFormatter={(val) => {
                                if (Math.abs(val) >= 1000000)
                                    return `£${(val / 1000000).toFixed(1)}M`;
                                return `£${(val / 1000).toFixed(0)}k`;
                            }}
                            tick={{ fontSize: 11, fill: "#94a3b8" }}
                            tickLine={false}
                            axisLine={false}
                            width={60}
                        />
                        <Tooltip
                            content={({ active, payload }) => {
                                if (!active || !payload?.length) return null;
                                const d = payload[0]
                                    ?.payload as ProjectionPoint;
                                if (!d) return null;
                                const hasEvents = d.events?.length > 0;
                                return (
                                    <div className="bg-slate-800 border border-slate-600 rounded-lg p-3 shadow-xl text-xs min-w-[180px]">
                                        <div className="font-semibold text-slate-100 mb-2 border-b border-slate-700 pb-1.5">
                                            Age {d.age}
                                        </div>
                                        {hasEvents && (
                                            <div className="mb-2 pb-2 border-b border-slate-700">
                                                {d.events.map((event, idx) => (
                                                    <div
                                                        key={idx}
                                                        className="text-amber-300 font-medium"
                                                    >
                                                        {event}
                                                    </div>
                                                ))}
                                            </div>
                                        )}
                                        <div className="space-y-1 mb-2">
                                            <div className="flex justify-between">
                                                <span className="text-slate-400">
                                                    Cash
                                                </span>
                                                <span
                                                    style={{
                                                        color: CATEGORY_CONFIG
                                                            .cash.color,
                                                    }}
                                                >
                                                    {formatCurrency(d.cash)}
                                                </span>
                                            </div>
                                            <div className="flex justify-between">
                                                <span className="text-slate-400">
                                                    Investments
                                                </span>
                                                <span
                                                    style={{
                                                        color: CATEGORY_CONFIG
                                                            .investments.color,
                                                    }}
                                                >
                                                    {formatCurrency(
                                                        d.investments,
                                                    )}
                                                </span>
                                            </div>
                                            <div className="flex justify-between">
                                                <span className="text-slate-400">
                                                    Property
                                                </span>
                                                <span
                                                    style={{
                                                        color: CATEGORY_CONFIG
                                                            .property.color,
                                                    }}
                                                >
                                                    {formatCurrency(d.property)}
                                                </span>
                                            </div>
                                            <div className="flex justify-between">
                                                <span className="text-slate-400">
                                                    Pension
                                                </span>
                                                <span
                                                    style={{
                                                        color: CATEGORY_CONFIG
                                                            .pension.color,
                                                    }}
                                                >
                                                    {formatCurrency(d.pension)}
                                                </span>
                                            </div>
                                            <div className="flex justify-between">
                                                <span className="text-slate-400">
                                                    Liabilities
                                                </span>
                                                <span
                                                    style={{
                                                        color: CATEGORY_CONFIG
                                                            .liabilities.color,
                                                    }}
                                                >
                                                    {formatCurrency(
                                                        d.liabilities,
                                                    )}
                                                </span>
                                            </div>
                                        </div>
                                        <div className="pt-2 border-t border-slate-700 flex justify-between font-semibold">
                                            <span className="text-slate-100">
                                                Net Worth
                                            </span>
                                            <span className="text-emerald-400">
                                                {formatCurrency(d.netWorth)}
                                            </span>
                                        </div>
                                    </div>
                                );
                            }}
                        />
                        {/* Event markers */}
                        {eventMarkers.map((marker) => (
                            <ReferenceLine
                                key={marker.id}
                                x={marker.age}
                                stroke={marker.color}
                                strokeDasharray="4 4"
                                strokeOpacity={0.7}
                            />
                        ))}
                        {/* Stacked areas */}
                        <Area
                            type="monotone"
                            dataKey="liabilities"
                            stackId="1"
                            stroke={CATEGORY_CONFIG.liabilities.color}
                            fill="url(#liabilitiesGradient)"
                            strokeWidth={0}
                        />
                        <Area
                            type="monotone"
                            dataKey="cash"
                            stackId="2"
                            stroke={CATEGORY_CONFIG.cash.color}
                            fill="url(#cashGradient)"
                            strokeWidth={0}
                        />
                        <Area
                            type="monotone"
                            dataKey="investments"
                            stackId="2"
                            stroke={CATEGORY_CONFIG.investments.color}
                            fill="url(#investmentsGradient)"
                            strokeWidth={0}
                        />
                        <Area
                            type="monotone"
                            dataKey="property"
                            stackId="2"
                            stroke={CATEGORY_CONFIG.property.color}
                            fill="url(#propertyGradient)"
                            strokeWidth={0}
                        />
                        <Area
                            type="monotone"
                            dataKey="pension"
                            stackId="2"
                            stroke={CATEGORY_CONFIG.pension.color}
                            fill="url(#pensionGradient)"
                            strokeWidth={0}
                        />
                        {/* Net worth line */}
                        <Line
                            type="monotone"
                            dataKey="netWorth"
                            stroke={CATEGORY_CONFIG.netWorth.color}
                            strokeWidth={2.5}
                            dot={false}
                            activeDot={{
                                r: 6,
                                fill: CATEGORY_CONFIG.netWorth.color,
                                stroke: "#1e293b",
                                strokeWidth: 2,
                            }}
                        />
                    </ComposedChart>
                </ResponsiveContainer>

                {/* Legend */}
                <div className="flex flex-wrap justify-center gap-4 mt-4 text-xs">
                    {Object.entries(CATEGORY_CONFIG).map(([key, config]) => (
                        <div key={key} className="flex items-center gap-1.5">
                            <span
                                className={`w-3 h-3 rounded ${
                                    key === "netWorth"
                                        ? "border-2 bg-transparent"
                                        : ""
                                }`}
                                style={{
                                    backgroundColor:
                                        key === "netWorth"
                                            ? "transparent"
                                            : config.color,
                                    borderColor:
                                        key === "netWorth"
                                            ? config.color
                                            : undefined,
                                }}
                            />
                            <span className="text-slate-400">
                                {config.label}
                            </span>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
});

// GitHub-style feature slideshow component
const FeatureSlideshow = memo(function FeatureSlideshow() {
    const [activeTab, setActiveTab] = useState(0);
    const [isPaused, setIsPaused] = useState(false);
    const [progress, setProgress] = useState(0);
    const timerRef = useRef<ReturnType<typeof setInterval> | undefined>(
        undefined,
    );
    const progressRef = useRef<ReturnType<typeof setInterval> | undefined>(
        undefined,
    );

    // Reset progress when active tab or pause state changes (React recommended pattern)
    const [prevState, setPrevState] = useState({ activeTab: 0, isPaused: false });
    if (activeTab !== prevState.activeTab || isPaused !== prevState.isPaused) {
        setPrevState({ activeTab, isPaused });
        if (!isPaused) setProgress(0);
    }

    // Auto-advance timer with progress tracking
    useEffect(() => {
        if (isPaused) {
            if (progressRef.current) clearInterval(progressRef.current);
            return;
        }

        // Progress bar updates every 50ms for smooth animation
        progressRef.current = setInterval(() => {
            setProgress((prev) => Math.min(prev + 1, 100));
        }, 50);

        timerRef.current = setInterval(() => {
            setActiveTab((prev) => (prev + 1) % FEATURE_TABS.length);
            setProgress(0);
        }, 5000);

        return () => {
            if (timerRef.current) clearInterval(timerRef.current);
            if (progressRef.current) clearInterval(progressRef.current);
        };
    }, [isPaused, activeTab]);

    const handleTabClick = useCallback((index: number) => {
        setActiveTab(index);
        setProgress(0);
        // Timer will be reset by the useEffect due to activeTab change
    }, []);

    const togglePause = useCallback(() => setIsPaused((p) => !p), []);

    return (
        <div className="relative max-w-5xl mx-auto">
            {/* Glow behind */}
            <div className="absolute -inset-4 bg-gradient-to-r from-emerald-500/20 via-blue-500/15 to-emerald-500/20 rounded-2xl blur-2xl opacity-60" />

            {/* Tab buttons with progress indicators */}
            <div className="flex justify-center gap-1 p-1 bg-slate-900/50 backdrop-blur-sm border border-slate-800 rounded-lg w-fit mx-auto mb-6">
                {FEATURE_TABS.map((tab, index) => (
                    <button
                        key={tab.id}
                        onClick={() => handleTabClick(index)}
                        className={`relative px-4 sm:px-6 py-2 text-sm font-medium rounded-md transition-all overflow-hidden ${
                            index === activeTab
                                ? "bg-slate-800 text-white"
                                : "text-slate-400 hover:text-white hover:bg-slate-800/50"
                        }`}
                    >
                        {tab.label}
                        {/* Progress bar for active tab */}
                        {index === activeTab && !isPaused && (
                            <div
                                className="absolute bottom-0 left-0 h-0.5 bg-emerald-500 transition-all duration-75 ease-linear"
                                style={{ width: `${progress}%` }}
                            />
                        )}
                    </button>
                ))}
            </div>

            {/* Main preview container */}
            <div className="relative">
                {/* Pause button */}
                <button
                    onClick={togglePause}
                    className="absolute top-4 right-4 z-20 p-2 bg-slate-900/80 backdrop-blur-sm border border-slate-700 rounded-md text-slate-400 hover:text-white transition-colors"
                    aria-label={isPaused ? "Play" : "Pause"}
                >
                    {isPaused ? <Play size={16} /> : <Pause size={16} />}
                </button>

                {/* Image container */}
                <div className="relative rounded-xl overflow-hidden border border-slate-700/50 shadow-2xl shadow-black/50 bg-slate-900">
                    {FEATURE_TABS.map((tab, index) => (
                        <img
                            key={tab.id}
                            src={tab.image}
                            alt={tab.label}
                            className={`w-full h-auto transition-all duration-700 ${
                                index === 0 ? "relative" : "absolute inset-0"
                            } ${index === activeTab ? "opacity-100" : "opacity-0"}`}
                        />
                    ))}
                </div>
            </div>

            {/* Tab description - below image */}
            <p className="text-center text-slate-400 max-w-xl mx-auto mt-6">
                {FEATURE_TABS[activeTab].description}
            </p>
        </div>
    );
});

// Infinite scrolling bank logo carousel
const BankLogoCarousel = memo(function BankLogoCarousel() {
    // Duplicate the logos for seamless infinite scroll
    const duplicatedLogos = [...BANK_LOGOS, ...BANK_LOGOS];

    return (
        <div className="relative overflow-hidden py-8">
            {/* Gradient fade edges */}
            <div className="absolute left-0 top-0 bottom-0 w-24 bg-gradient-to-r from-slate-950 to-transparent z-10 pointer-events-none" />
            <div className="absolute right-0 top-0 bottom-0 w-24 bg-gradient-to-l from-slate-950 to-transparent z-10 pointer-events-none" />

            {/* Scrolling container */}
            <div
                className="flex gap-12 animate-scroll"
                style={{
                    width: "fit-content",
                }}
            >
                {duplicatedLogos.map((bank, index) => (
                    <div
                        key={`${bank.name}-${index}`}
                        className="flex-shrink-0 flex flex-col items-center justify-center gap-2 px-4 opacity-70 hover:opacity-100 transition-all duration-300"
                    >
                        <img
                            src={bank.logo}
                            alt={bank.name}
                            className="h-10 w-10 object-contain"
                            loading="lazy"
                            onError={(e) => {
                                // Hide image if it fails to load
                                const target = e.target as HTMLImageElement;
                                target.style.display = "none";
                            }}
                        />
                        <span className="text-slate-400 font-semibold text-xs whitespace-nowrap">
                            {bank.name}
                        </span>
                    </div>
                ))}
            </div>

            {/* CSS for infinite scroll animation */}
            <style>{`
                @keyframes scroll {
                    0% {
                        transform: translateX(0);
                    }
                    100% {
                        transform: translateX(-50%);
                    }
                }
                .animate-scroll {
                    animation: scroll 60s linear infinite;
                }
            `}</style>
        </div>
    );
});

export default function Landing({ onGetStarted, onSignIn }: LandingPageProps) {
    // Scenario state
    const [scenarios, setScenarios] = useState<Scenario[]>([
        {
            id: "buyHouse",
            label: "Buy a House",
            icon: Home,
            color: "#3B82F6",
            enabled: true,
            age: 32,
            minAge: 26,
            maxAge: 45,
            description: "£350k property, £70k deposit",
        },
        {
            id: "haveKids",
            label: "Have Children",
            icon: Baby,
            color: "#EC4899",
            enabled: false,
            age: 34,
            minAge: 27,
            maxAge: 45,
            description: "£1,200/month for 18 years",
        },
        {
            id: "careerBreak",
            label: "Career Break",
            icon: Briefcase,
            color: "#F59E0B",
            enabled: false,
            age: 40,
            minAge: 28,
            maxAge: 55,
            description: "1 year sabbatical",
        },
    ]);

    const handleToggleScenario = useCallback((id: string) => {
        setScenarios((prev) =>
            prev.map((s) => (s.id === id ? { ...s, enabled: !s.enabled } : s)),
        );
    }, []);

    const handleChangeAge = useCallback((id: string, age: number) => {
        setScenarios((prev) =>
            prev.map((s) => (s.id === id ? { ...s, age } : s)),
        );
    }, []);

    const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

    return (
        <div className="min-h-screen bg-slate-950 overflow-x-clip relative">
            {/* Subtle background pattern */}
            <div className="fixed inset-0 opacity-[0.015] pointer-events-none">
                <div
                    className="absolute inset-0"
                    style={{
                        backgroundImage: `radial-gradient(circle at 1px 1px, rgba(16, 185, 129, 0.4) 1px, transparent 0)`,
                        backgroundSize: "40px 40px",
                    }}
                ></div>
            </div>

            {/* Background glow effects */}
            <div className="fixed inset-0 pointer-events-none overflow-hidden">
                <div className="absolute top-0 left-1/2 -translate-x-1/2 w-[1000px] h-[600px] bg-gradient-to-b from-emerald-500/10 via-blue-500/5 to-transparent rounded-full blur-3xl"></div>
            </div>

            {/* Header */}
            <header className="fixed top-0 left-0 right-0 z-50 bg-slate-950/80 backdrop-blur-xl border-b border-slate-800/50">
                <div className="max-w-[1280px] mx-auto px-6 h-16 flex items-center justify-between">
                    <div className="flex items-center gap-2.5">
                        <img
                            src="/networth-icon.svg"
                            alt="NetWorth"
                            className="w-8 h-8"
                        />
                        <span className="text-xl font-black tracking-tight text-white">
                            NetWorth
                        </span>
                    </div>
                    {/* Desktop navigation */}
                    <div className="hidden sm:flex items-center gap-3">
                        <a
                            href="https://networth.tbushell.co.uk"
                            target="_blank"
                            rel="noopener noreferrer"
                            className="text-sm font-medium text-slate-400 hover:text-white transition-colors px-3 py-2"
                        >
                            Docs
                        </a>
                        <button
                            onClick={onSignIn}
                            className="text-sm font-medium text-slate-300 hover:text-white transition-colors px-4 py-2"
                        >
                            Sign in
                        </button>
                        <button
                            onClick={onGetStarted}
                            className="text-sm font-semibold bg-white hover:bg-slate-100 text-slate-900 px-4 py-2 rounded-md transition-all hover:scale-105 active:scale-95"
                        >
                            Sign up
                        </button>
                    </div>
                    {/* Mobile navigation */}
                    <div className="flex sm:hidden items-center gap-2">
                        <button
                            onClick={onGetStarted}
                            className="text-sm font-semibold bg-emerald-500 hover:bg-emerald-600 text-white px-4 py-2 rounded-md transition-all"
                        >
                            Get started
                        </button>
                        <button
                            onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
                            className="p-2 text-slate-400 hover:text-white transition-colors"
                            aria-label={
                                mobileMenuOpen ? "Close menu" : "Open menu"
                            }
                        >
                            {mobileMenuOpen ? (
                                <X size={20} />
                            ) : (
                                <Menu size={20} />
                            )}
                        </button>
                    </div>
                </div>

                {/* Mobile menu dropdown */}
                {mobileMenuOpen && (
                    <div className="sm:hidden border-t border-slate-800/50 bg-slate-950/95 backdrop-blur-xl">
                        <div className="px-6 py-4 flex flex-col gap-3">
                            <a
                                href="https://networth.tbushell.co.uk"
                                target="_blank"
                                rel="noopener noreferrer"
                                className="text-sm font-medium text-slate-400 hover:text-white transition-colors py-2"
                                onClick={() => setMobileMenuOpen(false)}
                            >
                                Docs
                            </a>
                            <button
                                onClick={() => {
                                    onSignIn();
                                    setMobileMenuOpen(false);
                                }}
                                className="text-sm font-medium text-slate-300 hover:text-white transition-colors py-2 text-left"
                            >
                                Sign in
                            </button>
                            <button
                                onClick={() => {
                                    onGetStarted();
                                    setMobileMenuOpen(false);
                                }}
                                className="text-sm font-semibold bg-white hover:bg-slate-100 text-slate-900 px-4 py-2.5 rounded-md transition-all text-center"
                            >
                                Sign up
                            </button>
                        </div>
                    </div>
                )}
            </header>

            {/* Hero Section - GitHub Style */}
            <section className="relative pt-32 pb-8 px-6">
                <div className="max-w-[1280px] mx-auto">
                    {/* Hero Text - Centered */}
                    <div className="text-center max-w-4xl mx-auto mb-12">
                        <h1 className="text-4xl sm:text-5xl md:text-6xl lg:text-7xl font-black tracking-tight leading-[1.1] mb-6 text-white">
                            Know your wealth.
                            <br />
                            <span className="bg-gradient-to-r from-emerald-400 via-teal-400 to-blue-400 bg-clip-text text-transparent">
                                Build your future.
                            </span>
                        </h1>
                        <p className="text-lg md:text-xl text-slate-400 mb-8 max-w-2xl mx-auto leading-relaxed">
                            Track your net worth across all accounts in one
                            place. Connect your banks, see your complete
                            financial picture, and make smarter money decisions.
                        </p>

                        {/* CTA Button */}
                        <button
                            onClick={onGetStarted}
                            className="group h-12 px-8 text-base font-semibold bg-emerald-500 hover:bg-emerald-600 text-white rounded-md transition-all hover:scale-105 active:scale-95 hover:shadow-lg hover:shadow-emerald-500/25"
                        >
                            <span className="flex items-center gap-2">
                                Get started
                                <ChevronRight
                                    size={18}
                                    className="group-hover:translate-x-1 transition-transform"
                                />
                            </span>
                        </button>
                    </div>

                    {/* Interactive Projection Demo */}
                    <div className="relative max-w-5xl mx-auto mt-12">
                        {/* Glow behind */}
                        <div className="absolute -inset-4 bg-gradient-to-r from-emerald-500/20 via-blue-500/15 to-emerald-500/20 rounded-2xl blur-2xl opacity-60"></div>

                        {/* Main demo container */}
                        <div className="relative">
                            <div className="text-center mb-6">
                                <h2 className="text-xl md:text-2xl font-bold text-white mb-2">
                                    See how life decisions impact your wealth
                                </h2>
                                <p className="text-sm text-slate-400">
                                    Toggle scenarios and adjust ages to explore
                                    your financial future
                                </p>
                            </div>

                            <InteractiveProjectionChart
                                scenarios={scenarios}
                                onToggleScenario={handleToggleScenario}
                                onChangeAge={handleChangeAge}
                            />

                            {/* CTA under projection */}
                            <div className="text-center mt-8">
                                <button
                                    onClick={onGetStarted}
                                    className="h-11 px-6 text-sm font-semibold bg-emerald-500 hover:bg-emerald-600 text-white rounded-md transition-all"
                                >
                                    Start planning your future
                                </button>
                                <p className="text-xs text-slate-500 mt-3">
                                    Free to use • No credit card required
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            {/* Why Track Section - Card Grid */}
            <section className="py-24 px-6 border-t border-slate-800/50">
                <div className="max-w-[1280px] mx-auto">
                    <div className="text-center mb-12">
                        <h2 className="text-3xl md:text-4xl font-bold tracking-tight mb-4 text-white">
                            Why tracking your net worth matters
                        </h2>
                        <p className="text-lg text-slate-400 max-w-2xl mx-auto">
                            The foundation of wealth isn't luck. It's awareness.
                        </p>
                    </div>

                    <div className="grid sm:grid-cols-2 lg:grid-cols-4 gap-6">
                        <div className="group p-6 rounded-xl bg-slate-900/50 border border-slate-800 hover:border-emerald-500/50 transition-all duration-300 hover:-translate-y-1 hover:shadow-lg hover:shadow-emerald-500/10">
                            <div className="w-12 h-12 rounded-lg bg-emerald-500/10 flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
                                <Eye size={24} className="text-emerald-400" />
                            </div>
                            <h3 className="text-lg font-semibold text-white mb-2">
                                See the full picture
                            </h3>
                            <p className="text-sm text-slate-400">
                                Net worth is the only number that shows if
                                you're actually building wealth or just spinning
                                your wheels.
                            </p>
                        </div>

                        <div className="group p-6 rounded-xl bg-slate-900/50 border border-slate-800 hover:border-blue-500/50 transition-all duration-300 hover:-translate-y-1 hover:shadow-lg hover:shadow-blue-500/10">
                            <div className="w-12 h-12 rounded-lg bg-blue-500/10 flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
                                <TrendingUp
                                    size={24}
                                    className="text-blue-400"
                                />
                            </div>
                            <h3 className="text-lg font-semibold text-white mb-2">
                                Change your behavior
                            </h3>
                            <p className="text-sm text-slate-400">
                                Seeing the number daily changes decisions. That
                                £50 dinner becomes movement on your wealth
                                trajectory.
                            </p>
                        </div>

                        <div className="group p-6 rounded-xl bg-slate-900/50 border border-slate-800 hover:border-emerald-500/50 transition-all duration-300 hover:-translate-y-1 hover:shadow-lg hover:shadow-emerald-500/10">
                            <div className="w-12 h-12 rounded-lg bg-emerald-500/10 flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
                                <Target
                                    size={24}
                                    className="text-emerald-400"
                                />
                            </div>
                            <h3 className="text-lg font-semibold text-white mb-2">
                                Create momentum
                            </h3>
                            <p className="text-sm text-slate-400">
                                Concrete milestones replace vague goals. £100k
                                net worth. £250k. £1M. Progress becomes
                                tangible.
                            </p>
                        </div>

                        <div className="group p-6 rounded-xl bg-slate-900/50 border border-slate-800 hover:border-blue-500/50 transition-all duration-300 hover:-translate-y-1 hover:shadow-lg hover:shadow-blue-500/10">
                            <div className="w-12 h-12 rounded-lg bg-blue-500/10 flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
                                <BarChart3
                                    size={24}
                                    className="text-blue-400"
                                />
                            </div>
                            <h3 className="text-lg font-semibold text-white mb-2">
                                Spot patterns
                            </h3>
                            <p className="text-sm text-slate-400">
                                Track long enough and you'll see the invisible:
                                spending spikes, account creep, compound
                                effects.
                            </p>
                        </div>
                    </div>
                </div>
            </section>

            {/* Bank Logos Carousel */}
            <section className="py-16 px-6 bg-gradient-to-b from-slate-900/50 to-transparent">
                <div className="max-w-[1280px] mx-auto">
                    {/* Banner */}
                    <div className="text-center mb-8">
                        <h3 className="text-2xl md:text-3xl font-bold text-white mb-2">
                            Connect with 2,500+ banks
                        </h3>
                        <p className="text-slate-400 text-base max-w-lg mx-auto">
                            Securely link all your accounts with read-only
                            access. From high-street banks to digital
                            challengers.
                        </p>
                    </div>
                    <BankLogoCarousel />
                </div>
            </section>

            {/* Feature Slideshow - GitHub Style */}
            <section className="py-24 px-6">
                <div className="max-w-[1280px] mx-auto">
                    <FeatureSlideshow />
                </div>
            </section>

            {/* Trusted By / Stats Section */}
            <section className="py-16 px-6 border-t border-slate-800/50">
                <div className="max-w-[1280px] mx-auto">
                    <div className="grid grid-cols-2 md:grid-cols-4 gap-8 text-center">
                        <div>
                            <div className="text-3xl md:text-4xl font-bold text-white mb-1">
                                2,500+
                            </div>
                            <div className="text-sm text-slate-500">
                                Banks supported
                            </div>
                        </div>
                        <div>
                            <div className="text-3xl md:text-4xl font-bold text-white mb-1">
                                256-bit
                            </div>
                            <div className="text-sm text-slate-500">
                                Encryption
                            </div>
                        </div>
                        <div>
                            <div className="text-3xl md:text-4xl font-bold text-white mb-1">
                                £847
                            </div>
                            <div className="text-sm text-slate-500">
                                Avg. annual savings
                            </div>
                        </div>
                        <div>
                            <div className="text-3xl md:text-4xl font-bold text-white mb-1">
                                3.2x
                            </div>
                            <div className="text-sm text-slate-500">
                                More likely to hit goals
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            {/* CTA Section */}
            <section className="py-24 px-6 relative overflow-hidden">
                {/* Animated background elements */}
                <div className="absolute inset-0 pointer-events-none">
                    <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[800px] h-[800px] bg-emerald-500/10 rounded-full blur-3xl" />
                    <div className="absolute top-0 right-0 w-[400px] h-[400px] bg-blue-500/10 rounded-full blur-3xl" />
                    <div className="absolute bottom-0 left-0 w-[400px] h-[400px] bg-purple-500/10 rounded-full blur-3xl" />
                </div>

                <div className="max-w-[1280px] mx-auto relative">
                    <div className="grid lg:grid-cols-2 gap-12 items-center">
                        {/* Left side - Content */}
                        <div className="text-center lg:text-left">
                            <div className="inline-flex items-center gap-2 px-4 py-2 rounded-full bg-emerald-500/10 border border-emerald-500/20 mb-6">
                                <Sparkles
                                    size={16}
                                    className="text-emerald-400"
                                />
                                <span className="text-sm font-medium text-emerald-400">
                                    Free forever for personal use
                                </span>
                            </div>

                            <h2 className="text-4xl md:text-5xl lg:text-6xl font-black tracking-tight mb-6 text-white leading-tight">
                                Your wealth journey
                                <br />
                                <span className="bg-gradient-to-r from-emerald-400 to-blue-400 bg-clip-text text-transparent">
                                    starts here
                                </span>
                            </h2>

                            <p className="text-lg text-slate-400 mb-8 max-w-lg">
                                Join thousands who've taken control of their
                                finances. Connect your accounts in minutes and
                                see your complete financial picture.
                            </p>

                            <div className="flex flex-col sm:flex-row gap-4 justify-center lg:justify-start mb-8">
                                <button
                                    onClick={onGetStarted}
                                    className="group h-14 px-8 text-base font-semibold bg-emerald-500 hover:bg-emerald-600 text-white rounded-xl transition-all hover:scale-105 active:scale-95 hover:shadow-xl hover:shadow-emerald-500/25"
                                >
                                    <span className="flex items-center justify-center gap-2">
                                        Create free account
                                        <ChevronRight
                                            size={20}
                                            className="group-hover:translate-x-1 transition-transform"
                                        />
                                    </span>
                                </button>
                                <button
                                    onClick={onSignIn}
                                    className="h-14 px-8 text-base font-semibold text-slate-300 hover:text-white border border-slate-700 hover:border-slate-600 rounded-xl transition-all hover:bg-slate-800/50"
                                >
                                    Sign in
                                </button>
                            </div>

                            <div className="flex flex-wrap gap-6 justify-center lg:justify-start text-sm text-slate-500">
                                <div className="flex items-center gap-2">
                                    <svg
                                        className="w-5 h-5 text-emerald-500"
                                        fill="currentColor"
                                        viewBox="0 0 20 20"
                                    >
                                        <path
                                            fillRule="evenodd"
                                            d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
                                            clipRule="evenodd"
                                        />
                                    </svg>
                                    <span>No credit card</span>
                                </div>
                                <div className="flex items-center gap-2">
                                    <svg
                                        className="w-5 h-5 text-emerald-500"
                                        fill="currentColor"
                                        viewBox="0 0 20 20"
                                    >
                                        <path
                                            fillRule="evenodd"
                                            d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
                                            clipRule="evenodd"
                                        />
                                    </svg>
                                    <span>Bank-grade security</span>
                                </div>
                                <div className="flex items-center gap-2">
                                    <svg
                                        className="w-5 h-5 text-emerald-500"
                                        fill="currentColor"
                                        viewBox="0 0 20 20"
                                    >
                                        <path
                                            fillRule="evenodd"
                                            d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
                                            clipRule="evenodd"
                                        />
                                    </svg>
                                    <span>Read-only access</span>
                                </div>
                            </div>
                        </div>

                        {/* Right side - Visual element */}
                        <div className="hidden lg:block relative">
                            <div className="relative">
                                {/* Mock dashboard card */}
                                <div className="bg-slate-800/80 backdrop-blur-sm border border-slate-700 rounded-2xl p-6 shadow-2xl transform rotate-2 hover:rotate-0 transition-transform duration-500">
                                    <div className="flex items-center justify-between mb-6">
                                        <div>
                                            <p className="text-sm text-slate-400">
                                                Total Net Worth
                                            </p>
                                            <p className="text-3xl font-bold text-white">
                                                £247,832
                                            </p>
                                        </div>
                                        <div className="flex items-center gap-1 text-emerald-400 text-sm font-medium">
                                            <TrendingUp size={16} />
                                            <span>+12.4%</span>
                                        </div>
                                    </div>

                                    {/* Mini chart visualization */}
                                    <div className="h-24 flex items-end gap-1">
                                        {[
                                            35, 42, 38, 55, 48, 62, 58, 70, 65,
                                            78, 72, 85,
                                        ].map((height, i) => (
                                            <div
                                                key={i}
                                                className="flex-1 bg-gradient-to-t from-emerald-500 to-emerald-400 rounded-t opacity-80"
                                                style={{ height: `${height}%` }}
                                            />
                                        ))}
                                    </div>

                                    <div className="mt-6 grid grid-cols-3 gap-4">
                                        <div className="text-center p-3 bg-slate-900/50 rounded-lg">
                                            <p className="text-xs text-slate-500 mb-1">
                                                Cash
                                            </p>
                                            <p className="text-sm font-semibold text-white">
                                                £18,420
                                            </p>
                                        </div>
                                        <div className="text-center p-3 bg-slate-900/50 rounded-lg">
                                            <p className="text-xs text-slate-500 mb-1">
                                                Investments
                                            </p>
                                            <p className="text-sm font-semibold text-white">
                                                £89,412
                                            </p>
                                        </div>
                                        <div className="text-center p-3 bg-slate-900/50 rounded-lg">
                                            <p className="text-xs text-slate-500 mb-1">
                                                Property
                                            </p>
                                            <p className="text-sm font-semibold text-white">
                                                £140,000
                                            </p>
                                        </div>
                                    </div>
                                </div>

                                {/* Floating account cards */}
                                <div className="absolute -top-4 -left-8 bg-slate-800 border border-slate-700 rounded-xl p-3 shadow-xl transform -rotate-6">
                                    <div className="flex items-center gap-3">
                                        <div className="w-8 h-8 rounded-full bg-blue-500 flex items-center justify-center text-white text-xs font-bold">
                                            B
                                        </div>
                                        <div>
                                            <p className="text-xs text-slate-400">
                                                Barclays
                                            </p>
                                            <p className="text-sm font-semibold text-white">
                                                £4,230
                                            </p>
                                        </div>
                                    </div>
                                </div>

                                <div className="absolute -bottom-4 -right-4 bg-slate-800 border border-slate-700 rounded-xl p-3 shadow-xl transform rotate-3">
                                    <div className="flex items-center gap-3">
                                        <div className="w-8 h-8 rounded-full bg-orange-500 flex items-center justify-center text-white text-xs font-bold">
                                            M
                                        </div>
                                        <div>
                                            <p className="text-xs text-slate-400">
                                                Monzo
                                            </p>
                                            <p className="text-sm font-semibold text-white">
                                                £1,847
                                            </p>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            {/* Footer */}
            <footer className="py-12 px-6 border-t border-slate-800/50">
                <div className="max-w-[1280px] mx-auto">
                    <div className="flex flex-col md:flex-row justify-between items-center gap-6 mb-6">
                        <div className="flex items-center gap-2">
                            <img
                                src="/networth-icon.svg"
                                alt="NetWorth"
                                className="w-6 h-6"
                            />
                            <span className="text-lg font-bold text-white">
                                NetWorth
                            </span>
                        </div>
                        <div className="flex items-center gap-6 text-sm text-slate-400">
                            <a
                                href="https://networth.tbushell.co.uk"
                                target="_blank"
                                rel="noopener noreferrer"
                                className="hover:text-white transition-colors"
                            >
                                Documentation
                            </a>
                            <a
                                href="https://github.com/TomSB1423/NetWorth"
                                target="_blank"
                                rel="noopener noreferrer"
                                className="hover:text-white transition-colors"
                            >
                                GitHub
                            </a>
                        </div>
                    </div>
                    <div className="pt-6 border-t border-slate-800/50 flex flex-col md:flex-row justify-between items-center gap-4">
                        <p className="text-sm text-slate-500">
                            © 2026 NetWorth. All rights reserved.
                        </p>
                        <p className="text-xs text-slate-600">
                            Bank-grade security • Read-only access • Your data,
                            your control
                        </p>
                    </div>
                </div>
            </footer>
        </div>
    );
}
