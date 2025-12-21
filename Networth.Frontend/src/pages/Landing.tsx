import {
    TrendingUp,
    Eye,
    Target,
    Lightbulb,
    LineChart,
    Coins,
    ArrowRight,
} from "lucide-react";

interface LandingPageProps {
    onGetStarted: () => void;
    onSignIn: () => void;
}

export default function Landing({ onGetStarted, onSignIn }: LandingPageProps) {
    return (
        <div className="min-h-screen bg-slate-950 overflow-x-hidden relative">
            {/* Subtle background pattern */}
            <div className="fixed inset-0 opacity-[0.02] pointer-events-none">
                <div
                    className="absolute inset-0"
                    style={{
                        backgroundImage: `repeating-linear-gradient(
                            45deg,
                            transparent,
                            transparent 40px,
                            rgba(16, 185, 129, 0.5) 40px,
                            rgba(16, 185, 129, 0.5) 42px
                        ),
                        repeating-linear-gradient(
                            -45deg,
                            transparent,
                            transparent 40px,
                            rgba(59, 130, 246, 0.3) 40px,
                            rgba(59, 130, 246, 0.3) 42px
                        )`,
                    }}
                ></div>
            </div>

            {/* Header */}
            <header className="fixed top-0 left-0 right-0 z-50 bg-slate-950/60 backdrop-blur-2xl border-b border-slate-800/50">
                <div className="max-w-[1200px] mx-auto px-6 h-14 flex items-center justify-between">
                    <div className="flex items-center gap-2.5">
                        <img src="/networth-icon.svg" alt="NetWorth" className="w-7 h-7" />
                        <span className="text-lg font-black tracking-tight text-white">
                            NetWorth
                        </span>
                    </div>
                    <div className="flex items-center gap-3">
                        <button
                            onClick={onSignIn}
                            className="text-sm font-semibold text-slate-300 hover:text-emerald-400 transition-all h-9 px-4"
                        >
                            Sign in
                        </button>
                        <button
                            onClick={onGetStarted}
                            className="text-sm font-semibold bg-emerald-500 hover:bg-emerald-600 text-white h-9 px-4 rounded-lg transition-all"
                        >
                            Get Started
                        </button>
                    </div>
                </div>
            </header>

            {/* Hero Section */}
            <section className="relative pt-20 pb-12 px-6">
                <div className="absolute top-20 right-0 w-[400px] h-[400px] bg-gradient-to-br from-emerald-500/15 via-blue-500/8 to-transparent rounded-full blur-3xl"></div>
                <div className="max-w-[1200px] mx-auto relative">
                    <div className="max-w-4xl">
                        <div className="inline-block px-2.5 py-0.5 rounded-full bg-emerald-500/10 border border-emerald-500/20 mb-3">
                            <span className="text-xs font-bold text-emerald-400">
                                The Algorithm of Wealth
                            </span>
                        </div>
                        <h1 className="text-4xl md:text-6xl font-black tracking-tighter leading-[0.95] mb-3 text-white">
                            Know your
                            <br />
                            <span className="bg-gradient-to-r from-emerald-400 via-blue-400 to-emerald-400 bg-clip-text text-transparent">
                                number
                            </span>
                        </h1>
                        <p className="text-lg md:text-xl text-slate-400 max-w-2xl mb-5 leading-snug">
                            Most people have no idea what they're worth. You
                            can't build wealth if you don't measure it.
                        </p>
                        <div className="flex flex-col sm:flex-row gap-3">
                            <button
                                onClick={onGetStarted}
                                className="h-11 px-6 text-sm font-bold bg-gradient-to-r from-emerald-500 to-blue-500 hover:opacity-90 transition-opacity text-white rounded-lg flex items-center justify-center"
                            >
                                Start tracking free
                                <ArrowRight size={18} className="ml-2" />
                            </button>
                            <button className="h-11 px-6 text-sm font-bold border border-slate-700 hover:bg-slate-800/50 text-slate-300 rounded-lg transition-all">
                                See how it works
                            </button>
                        </div>
                    </div>
                </div>
            </section>

            {/* Why Tracking Matters Section */}
            <section className="py-12 px-6 relative">
                <div className="max-w-[1200px] mx-auto relative">
                    <div className="mb-6">
                        <h2 className="text-2xl md:text-3xl font-black tracking-tighter mb-1.5 text-white">
                            Why tracking matters
                        </h2>
                        <p className="text-base text-slate-400">
                            The foundation of wealth isn't luck or timing. It's
                            awareness.
                        </p>
                    </div>

                    <div className="grid md:grid-cols-2 gap-x-12 gap-y-6 max-w-5xl">
                        <div className="flex gap-3 pb-6 border-b border-slate-800/50">
                            <div className="flex-shrink-0 mt-1">
                                <Eye size={22} className="text-emerald-400" />
                            </div>
                            <div>
                                <h3 className="text-lg font-bold tracking-tight mb-1 text-white">
                                    You can't optimize what you don't see
                                </h3>
                                <p className="text-sm text-slate-400 leading-relaxed">
                                    Net worth is the only number that shows if
                                    you're actually building wealth or just
                                    spinning your wheels.
                                </p>
                            </div>
                        </div>

                        <div className="flex gap-3 pb-6 border-b border-slate-800/50">
                            <div className="flex-shrink-0 mt-1">
                                <TrendingUp
                                    size={22}
                                    className="text-blue-400"
                                />
                            </div>
                            <div>
                                <h3 className="text-lg font-bold tracking-tight mb-1 text-white">
                                    Behavior changes when you measure
                                </h3>
                                <p className="text-sm text-slate-400 leading-relaxed">
                                    Seeing the number daily changes decisions.
                                    That £50 dinner becomes movement on your
                                    wealth trajectory.
                                </p>
                            </div>
                        </div>

                        <div className="flex gap-3 pb-6 border-b border-slate-800/50">
                            <div className="flex-shrink-0 mt-1">
                                <Target
                                    size={22}
                                    className="text-emerald-400"
                                />
                            </div>
                            <div>
                                <h3 className="text-lg font-bold tracking-tight mb-1 text-white">
                                    Clarity creates momentum
                                </h3>
                                <p className="text-sm text-slate-400 leading-relaxed">
                                    Concrete milestones replace vague goals.
                                    £100k net worth. £250k. £1M. Progress
                                    becomes tangible.
                                </p>
                            </div>
                        </div>

                        <div className="flex gap-3 pb-6 border-b border-slate-800/50">
                            <div className="flex-shrink-0 mt-1">
                                <Lightbulb
                                    size={22}
                                    className="text-blue-400"
                                />
                            </div>
                            <div>
                                <h3 className="text-lg font-bold tracking-tight mb-1 text-white">
                                    Patterns emerge from data
                                </h3>
                                <p className="text-sm text-slate-400 leading-relaxed">
                                    Track long enough and you'll spot the
                                    invisible: spending spikes, account creep,
                                    compound effects.
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            {/* Wealth Formula Section */}
            <section className="py-12 px-6 relative">
                <div className="max-w-[1200px] mx-auto relative">
                    <div className="border border-slate-800/50 rounded-lg p-6 md:p-8 bg-slate-900/30">
                        <div className="max-w-3xl mx-auto">
                            <h2 className="text-2xl md:text-3xl font-bold tracking-tight mb-2 text-white">
                                The wealth formula is simple
                            </h2>
                            <div className="my-4 py-4 px-5 rounded-lg bg-slate-950/50 border border-slate-800">
                                <p className="text-xl md:text-2xl font-bold tracking-tight text-white">
                                    Assets - Liabilities = Net Worth
                                </p>
                            </div>
                            <p className="text-base text-slate-400 mb-6">
                                The execution? That's where most people fail.
                                Not because it's hard, but because they never
                                start measuring. NetWorth makes it automatic.
                            </p>
                            <ul className="space-y-2.5 text-sm text-slate-400">
                                <li className="flex items-start gap-2.5">
                                    <div className="w-1.5 h-1.5 rounded-full bg-emerald-400 mt-1.5 flex-shrink-0"></div>
                                    <span>
                                        Connect all accounts in 60 seconds
                                    </span>
                                </li>
                                <li className="flex items-start gap-2.5">
                                    <div className="w-1.5 h-1.5 rounded-full bg-emerald-400 mt-1.5 flex-shrink-0"></div>
                                    <span>
                                        Automatic daily updates—no spreadsheets
                                    </span>
                                </li>
                                <li className="flex items-start gap-2.5">
                                    <div className="w-1.5 h-1.5 rounded-full bg-emerald-400 mt-1.5 flex-shrink-0"></div>
                                    <span>
                                        Visual trends that show if you're
                                        winning or stalling
                                    </span>
                                </li>
                                <li className="flex items-start gap-2.5">
                                    <div className="w-1.5 h-1.5 rounded-full bg-emerald-400 mt-1.5 flex-shrink-0"></div>
                                    <span>
                                        Goal tracking that turns vague hopes
                                        into concrete targets
                                    </span>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>
            </section>

            {/* Features Section */}
            <section className="py-12 px-6">
                <div className="max-w-[1200px] mx-auto">
                    <div className="grid lg:grid-cols-[1.2fr_1fr] gap-8 items-start">
                        <div>
                            <h2 className="text-2xl md:text-3xl font-bold tracking-tight mb-2 text-white">
                                Built for people who are serious
                            </h2>
                            <p className="text-sm text-slate-400 mb-6">
                                This isn't a budgeting app with cartoon pigs.
                                It's a wealth-tracking instrument for people who
                                understand that knowing your number is the first
                                step to changing it.
                            </p>
                            <div className="space-y-4 border-l-2 border-slate-800/50 pl-5">
                                <div>
                                    <div className="flex items-center gap-2.5 mb-1.5">
                                        <TrendingUp
                                            size={18}
                                            className="text-emerald-400"
                                        />
                                        <h4 className="font-black text-base text-white">
                                            Real-time net worth tracking
                                        </h4>
                                    </div>
                                    <p className="text-sm text-slate-400 leading-relaxed">
                                        See your wealth change as accounts
                                        update. No more guessing, no more
                                        spreadsheets.
                                    </p>
                                </div>
                                <div>
                                    <div className="flex items-center gap-2.5 mb-1.5">
                                        <Coins
                                            size={18}
                                            className="text-blue-400"
                                        />
                                        <h4 className="font-black text-base text-white">
                                            Multi-account aggregation
                                        </h4>
                                    </div>
                                    <p className="text-sm text-slate-400 leading-relaxed">
                                        Current accounts, savings, credit cards,
                                        loans. Everything in one dashboard.
                                    </p>
                                </div>
                                <div>
                                    <div className="flex items-center gap-2.5 mb-1.5">
                                        <Target
                                            size={18}
                                            className="text-emerald-400"
                                        />
                                        <h4 className="font-black text-base text-white">
                                            Milestone goal setting
                                        </h4>
                                    </div>
                                    <p className="text-sm text-slate-400 leading-relaxed">
                                        Set targets, track progress, celebrate
                                        wins. Turn abstract wealth into concrete
                                        achievements.
                                    </p>
                                </div>
                            </div>
                        </div>
                        <div className="relative">
                            <div className="absolute inset-0 bg-gradient-to-br from-emerald-500/10 via-blue-500/10 to-emerald-500/10 rounded-lg blur-2xl"></div>
                            <div className="relative rounded-lg bg-slate-900 border border-slate-800/50 p-6 min-h-[260px] flex items-center justify-center">
                                <div className="text-center">
                                    <div className="w-16 h-16 rounded-xl bg-gradient-to-br from-emerald-500 via-blue-500 to-emerald-500 flex items-center justify-center mx-auto mb-4">
                                        <LineChart
                                            size={32}
                                            className="text-white"
                                        />
                                    </div>
                                    <div className="space-y-2">
                                        <div className="h-1.5 bg-emerald-500/20 rounded-full w-3/4 mx-auto"></div>
                                        <div className="h-1.5 bg-blue-500/20 rounded-full w-1/2 mx-auto"></div>
                                        <div className="h-1.5 bg-emerald-500/20 rounded-full w-5/6 mx-auto"></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            {/* Stats Section */}
            <section className="py-12 px-6 relative">
                <div className="max-w-[1200px] mx-auto relative">
                    <div className="max-w-4xl mx-auto">
                        <h2 className="text-2xl md:text-3xl font-bold tracking-tight mb-2 text-white">
                            What you don't know is costing you
                        </h2>
                        <p className="text-sm text-slate-400 mb-6">
                            Every day without clarity is a day of missed
                            opportunities. Hidden fees. Forgotten subscriptions.
                            Accounts you opened years ago and never closed. The
                            cost of ignorance compounds silently.
                        </p>
                        <div className="grid md:grid-cols-3 gap-4 mb-6">
                            <div className="p-4 rounded-lg bg-slate-900/30 border border-slate-800/50">
                                <div className="text-2xl font-bold text-emerald-400 mb-1">
                                    £847
                                </div>
                                <p className="text-xs text-slate-400">
                                    Average annual savings when tracking net
                                    worth actively
                                </p>
                            </div>
                            <div className="p-4 rounded-lg bg-slate-900/30 border border-slate-800/50">
                                <div className="text-2xl font-bold text-blue-400 mb-1">
                                    32%
                                </div>
                                <p className="text-xs text-slate-400">
                                    Faster wealth growth with regular monitoring
                                </p>
                            </div>
                            <div className="p-4 rounded-lg bg-slate-900/30 border border-slate-800/50">
                                <div className="text-2xl font-bold text-emerald-400 mb-1">
                                    3.2x
                                </div>
                                <p className="text-xs text-slate-400">
                                    More likely to hit financial goals with
                                    tracking
                                </p>
                            </div>
                        </div>
                        <p className="text-sm text-slate-400 mb-5">
                            The algorithm of wealth isn't secret. It's measured,
                            monitored, and optimized. Start today.
                        </p>
                        <button
                            onClick={onGetStarted}
                            className="h-10 px-5 text-sm font-semibold bg-gradient-to-r from-emerald-500 to-blue-500 hover:opacity-90 transition-opacity text-white rounded-lg flex items-center"
                        >
                            Start tracking your wealth
                            <ArrowRight size={16} className="ml-2" />
                        </button>
                    </div>
                </div>
            </section>

            {/* Footer */}
            <footer className="py-8 px-6 border-t border-slate-800/50 bg-slate-900/20">
                <div className="max-w-[1200px] mx-auto">
                    <div className="flex flex-col md:flex-row justify-between items-center gap-4 mb-4">
                        <div className="flex items-center gap-2.5">
                            <div className="w-7 h-7 rounded-lg bg-gradient-to-br from-emerald-500 via-blue-500 to-emerald-500 flex items-center justify-center">
                                <LineChart size={18} className="text-white" />
                            </div>
                            <span className="text-lg font-black tracking-tight text-white">
                                NetWorth
                            </span>
                        </div>
                        <div className="flex gap-8 text-xs font-semibold text-slate-400">
                            <a
                                href="#"
                                className="hover:text-emerald-400 transition-colors"
                            >
                                Privacy
                            </a>
                            <a
                                href="#"
                                className="hover:text-emerald-400 transition-colors"
                            >
                                Terms
                            </a>
                            <a
                                href="#"
                                className="hover:text-emerald-400 transition-colors"
                            >
                                Security
                            </a>
                            <a
                                href="#"
                                className="hover:text-emerald-400 transition-colors"
                            >
                                Contact
                            </a>
                        </div>
                    </div>
                    <div className="text-center text-xs text-slate-500">
                        <p>
                            Bank-grade security. Read-only access. Your data,
                            your control.
                        </p>
                    </div>
                </div>
            </footer>
        </div>
    );
}
