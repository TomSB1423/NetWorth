import { useState, useEffect, useCallback, useRef } from "react";
import {
    TrendingUp,
    Eye,
    Target,
    ArrowRight,
    CreditCard,
    Landmark,
    PiggyBank,
    ChevronDown,
    Pause,
    Link2,
    Shield,
    BarChart3,
    RefreshCw,
    Bell,
    Lock,
} from "lucide-react";

interface LandingPageProps {
    onGetStarted: () => void;
    onSignIn: () => void;
}

// Feature tabs configuration
const featureTabs = [
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
];

// Accordion features configuration
const accordionFeatures = [
    {
        id: "aggregation",
        icon: Link2,
        title: "Aggregate all your accounts",
        description:
            "Connect checking, savings, credit cards, and loans from 2,500+ banks. One view of your entire financial life.",
        link: "#",
        linkText: "Learn about bank connections",
    },
    {
        id: "realtime",
        icon: RefreshCw,
        title: "Real-time balance updates",
        description:
            "Your balances sync automatically throughout the day. Always know exactly where you stand.",
        link: "#",
        linkText: "See how sync works",
    },
    {
        id: "alerts",
        icon: Bell,
        title: "Smart notifications",
        description:
            "Get notified about large transactions, balance changes, and when you hit your wealth milestones.",
        link: "#",
        linkText: "Explore notifications",
    },
    {
        id: "security",
        icon: Lock,
        title: "Bank-grade security",
        description:
            "256-bit encryption, read-only access, and SOC 2 compliance. Your data is never sold or shared.",
        link: "#",
        linkText: "Review our security",
    },
];

export default function Landing({ onGetStarted, onSignIn }: LandingPageProps) {
    const [activeTab, setActiveTab] = useState(0);
    const [isPaused, setIsPaused] = useState(false);
    const [expandedFeature, setExpandedFeature] = useState<string | null>(
        "aggregation"
    );
    const timerRef = useRef<ReturnType<typeof setInterval> | null>(null);

    // Reset and restart the auto-advance timer for tabs
    const resetTimer = useCallback(() => {
        if (timerRef.current) {
            clearInterval(timerRef.current);
        }
        if (!isPaused) {
            timerRef.current = setInterval(() => {
                setActiveTab((prev) => (prev + 1) % featureTabs.length);
            }, 5000);
        }
    }, [isPaused]);

    useEffect(() => {
        resetTimer();
        return () => {
            if (timerRef.current) {
                clearInterval(timerRef.current);
            }
        };
    }, [resetTimer]);

    const handleTabClick = (index: number) => {
        setActiveTab(index);
        resetTimer();
    };

    const togglePause = () => {
        setIsPaused(!isPaused);
    };

    const toggleFeature = (id: string) => {
        setExpandedFeature(expandedFeature === id ? null : id);
    };

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
                    <div className="flex items-center gap-3">
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
                            className="text-sm font-semibold bg-white hover:bg-slate-100 text-slate-900 px-4 py-2 rounded-md transition-all"
                        >
                            Sign up
                        </button>
                    </div>
                </div>
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
                            className="h-12 px-8 text-base font-semibold bg-emerald-500 hover:bg-emerald-600 text-white rounded-md transition-all"
                        >
                            Get started
                        </button>
                    </div>

                    {/* Feature Preview - Large centered image/video area */}
                    <div className="relative max-w-5xl mx-auto">
                        {/* Glow behind */}
                        <div className="absolute -inset-4 bg-gradient-to-r from-emerald-500/20 via-blue-500/15 to-emerald-500/20 rounded-2xl blur-2xl opacity-60"></div>

                        {/* Main preview container */}
                        <div className="relative">
                            {/* Pause button */}
                            <button
                                onClick={togglePause}
                                className="absolute top-4 right-4 z-20 p-2 bg-slate-900/80 backdrop-blur-sm border border-slate-700 rounded-md text-slate-400 hover:text-white transition-colors"
                                aria-label={isPaused ? "Play" : "Pause"}
                            >
                                {isPaused ? (
                                    <Play size={16} />
                                ) : (
                                    <Pause size={16} />
                                )}
                            </button>

                            {/* Image container */}
                            <div className="relative rounded-xl overflow-hidden border border-slate-700/50 shadow-2xl shadow-black/50 bg-slate-900">
                                {featureTabs.map((tab, index) => (
                                    <img
                                        key={tab.id}
                                        src={tab.image}
                                        alt={tab.label}
                                        className={`w-full h-auto transition-all duration-700 ${
                                            index === 0
                                                ? "relative"
                                                : "absolute inset-0"
                                        } ${
                                            index === activeTab
                                                ? "opacity-100"
                                                : "opacity-0"
                                        }`}
                                    />
                                ))}
                            </div>
                        </div>

                        {/* Feature tabs below image */}
                        <div className="mt-6">
                            {/* Tab buttons */}
                            <div className="flex justify-center gap-1 p-1 bg-slate-900/50 backdrop-blur-sm border border-slate-800 rounded-lg w-fit mx-auto mb-4">
                                {featureTabs.map((tab, index) => (
                                    <button
                                        key={tab.id}
                                        onClick={() => handleTabClick(index)}
                                        className={`px-4 sm:px-6 py-2 text-sm font-medium rounded-md transition-all ${
                                            index === activeTab
                                                ? "bg-slate-800 text-white"
                                                : "text-slate-400 hover:text-white"
                                        }`}
                                    >
                                        {tab.label}
                                    </button>
                                ))}
                            </div>

                            {/* Tab description */}
                            <p className="text-center text-slate-400 max-w-xl mx-auto">
                                {featureTabs[activeTab].description}
                            </p>
                        </div>
                    </div>
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

            {/* Features Accordion Section - GitHub Style */}
            <section id="features" className="py-24 px-6">
                <div className="max-w-[1280px] mx-auto">
                    <div className="grid lg:grid-cols-2 gap-12 lg:gap-20 items-start">
                        {/* Left: Heading and Accordion */}
                        <div>
                            <h2 className="text-3xl md:text-4xl lg:text-5xl font-bold tracking-tight mb-4 text-white leading-tight">
                                Everything you need to
                                <br />
                                <span className="text-emerald-400">
                                    master your money
                                </span>
                            </h2>
                            <p className="text-lg text-slate-400 mb-10">
                                From automatic account syncing to smart
                                insights, NetWorth gives you the tools to
                                understand and grow your wealth.
                            </p>

                            {/* Accordion */}
                            <div className="space-y-2">
                                {accordionFeatures.map((feature) => {
                                    const Icon = feature.icon;
                                    const isExpanded =
                                        expandedFeature === feature.id;
                                    return (
                                        <div
                                            key={feature.id}
                                            className={`border rounded-lg transition-all ${
                                                isExpanded
                                                    ? "border-emerald-500/50 bg-emerald-500/5"
                                                    : "border-slate-800 hover:border-slate-700"
                                            }`}
                                        >
                                            <button
                                                onClick={() =>
                                                    toggleFeature(feature.id)
                                                }
                                                className="w-full flex items-center justify-between p-4 text-left"
                                            >
                                                <div className="flex items-center gap-3">
                                                    <div
                                                        className={`w-10 h-10 rounded-lg flex items-center justify-center transition-colors ${
                                                            isExpanded
                                                                ? "bg-emerald-500/20"
                                                                : "bg-slate-800"
                                                        }`}
                                                    >
                                                        <Icon
                                                            size={20}
                                                            className={
                                                                isExpanded
                                                                    ? "text-emerald-400"
                                                                    : "text-slate-400"
                                                            }
                                                        />
                                                    </div>
                                                    <h3
                                                        className={`font-semibold ${
                                                            isExpanded
                                                                ? "text-white"
                                                                : "text-slate-300"
                                                        }`}
                                                    >
                                                        {feature.title}
                                                    </h3>
                                                </div>
                                                <ChevronDown
                                                    size={20}
                                                    className={`text-slate-500 transition-transform ${
                                                        isExpanded
                                                            ? "rotate-180"
                                                            : ""
                                                    }`}
                                                />
                                            </button>
                                            {isExpanded && (
                                                <div className="px-4 pb-4 pl-[68px]">
                                                    <p className="text-slate-400 text-sm mb-3">
                                                        {feature.description}
                                                    </p>
                                                    <a
                                                        href={feature.link}
                                                        className="inline-flex items-center gap-1 text-sm font-medium text-emerald-400 hover:text-emerald-300 transition-colors"
                                                    >
                                                        {feature.linkText}
                                                        <ArrowRight size={14} />
                                                    </a>
                                                </div>
                                            )}
                                        </div>
                                    );
                                })}
                            </div>
                        </div>

                        {/* Right: Feature visual */}
                        <div className="relative lg:sticky lg:top-32">
                            <div className="absolute -inset-4 bg-gradient-to-br from-emerald-500/10 via-blue-500/10 to-purple-500/10 rounded-2xl blur-2xl"></div>
                            <div className="relative bg-slate-900 border border-slate-800 rounded-xl p-8 overflow-hidden">
                                {/* Decorative elements */}
                                <div className="absolute top-0 right-0 w-40 h-40 bg-emerald-500/10 rounded-full blur-3xl"></div>
                                <div className="absolute bottom-0 left-0 w-32 h-32 bg-blue-500/10 rounded-full blur-3xl"></div>

                                {/* Content based on expanded feature */}
                                <div className="relative">
                                    {expandedFeature === "aggregation" && (
                                        <div className="space-y-4">
                                            <div className="flex items-center gap-3 p-3 bg-slate-800/50 rounded-lg border border-slate-700/50">
                                                <div className="w-10 h-10 rounded-lg bg-blue-500/20 flex items-center justify-center">
                                                    <Landmark className="w-5 h-5 text-blue-400" />
                                                </div>
                                                <div className="flex-1">
                                                    <div className="text-sm font-medium text-white">
                                                        Current Account
                                                    </div>
                                                    <div className="text-xs text-slate-500">
                                                        Barclays
                                                    </div>
                                                </div>
                                                <div className="text-right">
                                                    <div className="text-sm font-semibold text-emerald-400">
                                                        £4,250.00
                                                    </div>
                                                </div>
                                            </div>
                                            <div className="flex items-center gap-3 p-3 bg-slate-800/50 rounded-lg border border-slate-700/50">
                                                <div className="w-10 h-10 rounded-lg bg-emerald-500/20 flex items-center justify-center">
                                                    <PiggyBank className="w-5 h-5 text-emerald-400" />
                                                </div>
                                                <div className="flex-1">
                                                    <div className="text-sm font-medium text-white">
                                                        Savings
                                                    </div>
                                                    <div className="text-xs text-slate-500">
                                                        Marcus
                                                    </div>
                                                </div>
                                                <div className="text-right">
                                                    <div className="text-sm font-semibold text-emerald-400">
                                                        £12,500.00
                                                    </div>
                                                </div>
                                            </div>
                                            <div className="flex items-center gap-3 p-3 bg-slate-800/50 rounded-lg border border-slate-700/50">
                                                <div className="w-10 h-10 rounded-lg bg-red-500/20 flex items-center justify-center">
                                                    <CreditCard className="w-5 h-5 text-red-400" />
                                                </div>
                                                <div className="flex-1">
                                                    <div className="text-sm font-medium text-white">
                                                        Credit Card
                                                    </div>
                                                    <div className="text-xs text-slate-500">
                                                        Amex
                                                    </div>
                                                </div>
                                                <div className="text-right">
                                                    <div className="text-sm font-semibold text-red-400">
                                                        -£1,200.00
                                                    </div>
                                                </div>
                                            </div>
                                            <div className="pt-4 border-t border-slate-700/50">
                                                <div className="flex justify-between items-center">
                                                    <span className="text-slate-400">
                                                        Net Worth
                                                    </span>
                                                    <span className="text-2xl font-bold text-white">
                                                        £15,550.00
                                                    </span>
                                                </div>
                                            </div>
                                        </div>
                                    )}
                                    {expandedFeature === "realtime" && (
                                        <div className="space-y-4">
                                            <div className="flex items-center justify-between p-3 bg-emerald-500/10 border border-emerald-500/30 rounded-lg">
                                                <div className="flex items-center gap-2">
                                                    <RefreshCw
                                                        size={16}
                                                        className="text-emerald-400 animate-spin"
                                                        style={{
                                                            animationDuration:
                                                                "2s",
                                                        }}
                                                    />
                                                    <span className="text-sm text-emerald-400">
                                                        Syncing accounts...
                                                    </span>
                                                </div>
                                                <span className="text-xs text-slate-500">
                                                    Last sync: 2 min ago
                                                </span>
                                            </div>
                                            <div className="h-32 flex items-end gap-2">
                                                {[
                                                    40, 55, 45, 60, 50, 70, 65,
                                                ].map((h, i) => (
                                                    <div
                                                        key={i}
                                                        className="flex-1 bg-gradient-to-t from-emerald-500 to-emerald-400 rounded-t opacity-80"
                                                        style={{
                                                            height: `${h}%`,
                                                        }}
                                                    ></div>
                                                ))}
                                            </div>
                                            <div className="text-center">
                                                <div className="text-3xl font-bold text-white">
                                                    +£2,340
                                                </div>
                                                <div className="text-sm text-emerald-400">
                                                    This month
                                                </div>
                                            </div>
                                        </div>
                                    )}
                                    {expandedFeature === "alerts" && (
                                        <div className="space-y-3">
                                            <div className="p-3 bg-slate-800/50 rounded-lg border-l-2 border-emerald-500">
                                                <div className="text-xs text-slate-500 mb-1">
                                                    Just now
                                                </div>
                                                <div className="text-sm text-white">
                                                    🎉 Milestone reached! You've
                                                    hit £15,000 net worth
                                                </div>
                                            </div>
                                            <div className="p-3 bg-slate-800/50 rounded-lg border-l-2 border-blue-500">
                                                <div className="text-xs text-slate-500 mb-1">
                                                    2 hours ago
                                                </div>
                                                <div className="text-sm text-white">
                                                    💰 Salary deposited: £3,200
                                                    to Barclays
                                                </div>
                                            </div>
                                            <div className="p-3 bg-slate-800/50 rounded-lg border-l-2 border-yellow-500">
                                                <div className="text-xs text-slate-500 mb-1">
                                                    Yesterday
                                                </div>
                                                <div className="text-sm text-white">
                                                    ⚠️ Large transaction: £450
                                                    at Currys
                                                </div>
                                            </div>
                                        </div>
                                    )}
                                    {expandedFeature === "security" && (
                                        <div className="space-y-4 text-center">
                                            <div className="w-20 h-20 rounded-full bg-emerald-500/20 flex items-center justify-center mx-auto">
                                                <Shield
                                                    size={40}
                                                    className="text-emerald-400"
                                                />
                                            </div>
                                            <div>
                                                <div className="text-xl font-bold text-white mb-2">
                                                    Bank-grade security
                                                </div>
                                                <div className="text-sm text-slate-400">
                                                    Your data is encrypted in
                                                    transit and at rest with
                                                    256-bit AES encryption
                                                </div>
                                            </div>
                                            <div className="grid grid-cols-2 gap-3 pt-4">
                                                <div className="p-3 bg-slate-800/50 rounded-lg">
                                                    <Lock
                                                        size={20}
                                                        className="text-emerald-400 mx-auto mb-2"
                                                    />
                                                    <div className="text-xs text-slate-400">
                                                        Read-only access
                                                    </div>
                                                </div>
                                                <div className="p-3 bg-slate-800/50 rounded-lg">
                                                    <Shield
                                                        size={20}
                                                        className="text-emerald-400 mx-auto mb-2"
                                                    />
                                                    <div className="text-xs text-slate-400">
                                                        SOC 2 compliant
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    )}
                                </div>
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
                        <div className="p-6 rounded-xl bg-slate-900/50 border border-slate-800 hover:border-slate-700 transition-colors">
                            <div className="w-12 h-12 rounded-lg bg-emerald-500/10 flex items-center justify-center mb-4">
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

                        <div className="p-6 rounded-xl bg-slate-900/50 border border-slate-800 hover:border-slate-700 transition-colors">
                            <div className="w-12 h-12 rounded-lg bg-blue-500/10 flex items-center justify-center mb-4">
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

                        <div className="p-6 rounded-xl bg-slate-900/50 border border-slate-800 hover:border-slate-700 transition-colors">
                            <div className="w-12 h-12 rounded-lg bg-emerald-500/10 flex items-center justify-center mb-4">
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

                        <div className="p-6 rounded-xl bg-slate-900/50 border border-slate-800 hover:border-slate-700 transition-colors">
                            <div className="w-12 h-12 rounded-lg bg-blue-500/10 flex items-center justify-center mb-4">
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

            {/* CTA Section */}
            <section className="py-24 px-6">
                <div className="max-w-[1280px] mx-auto">
                    <div className="relative rounded-2xl overflow-hidden">
                        {/* Background */}
                        <div className="absolute inset-0 bg-gradient-to-br from-emerald-500/20 via-slate-900 to-blue-500/20"></div>
                        <div className="absolute inset-0 bg-slate-900/80"></div>

                        <div className="relative px-8 py-16 md:py-24 text-center">
                            <h2 className="text-3xl md:text-4xl lg:text-5xl font-bold tracking-tight mb-4 text-white">
                                Start tracking your wealth today
                            </h2>
                            <p className="text-lg text-slate-400 mb-8 max-w-xl mx-auto">
                                Join thousands who've taken control of their
                                finances. Free to start, powerful enough to grow
                                with you.
                            </p>
                            <button
                                onClick={onGetStarted}
                                className="h-12 px-8 text-base font-semibold bg-emerald-500 hover:bg-emerald-600 text-white rounded-md transition-all"
                            >
                                Get started
                            </button>
                            <p className="text-sm text-slate-500 mt-4">
                                No credit card required • Bank-grade security •
                                Cancel anytime
                            </p>
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
