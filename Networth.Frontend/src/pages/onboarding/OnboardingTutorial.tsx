import { useState } from "react";
import { useNavigate } from "react-router-dom";
import {
    ArrowRight,
    ArrowLeft,
    Shield,
    Lock,
    Globe,
    LineChart,
    LogOut,
    TrendingUp,
    PieChart,
    RefreshCw,
} from "lucide-react";
import { useAuth } from "../../contexts/AuthContext";

interface OnboardingSlide {
    icon: React.ReactNode;
    iconBg: string;
    title: string;
    description: string;
}

const slides: OnboardingSlide[] = [
    {
        icon: <LineChart className="w-12 h-12 text-blue-400" />,
        iconBg: "bg-blue-500/10",
        title: "Welcome to NetWorth",
        description:
            "Your complete financial picture in one place. Track your net worth across all your accounts in real-time.",
    },
    {
        icon: <Globe className="w-12 h-12 text-emerald-400" />,
        iconBg: "bg-emerald-500/10",
        title: "Connect Your Accounts",
        description:
            "Securely link your bank accounts, savings, and investments. We support thousands of financial institutions.",
    },
    {
        icon: <TrendingUp className="w-12 h-12 text-purple-400" />,
        iconBg: "bg-purple-500/10",
        title: "Track Your Progress",
        description:
            "Watch your net worth grow over time with beautiful charts and insights. Set goals and celebrate milestones.",
    },
    {
        icon: <PieChart className="w-12 h-12 text-orange-400" />,
        iconBg: "bg-orange-500/10",
        title: "Understand Your Finances",
        description:
            "See how your wealth is distributed across different accounts and asset types at a glance.",
    },
    {
        icon: <RefreshCw className="w-12 h-12 text-cyan-400" />,
        iconBg: "bg-cyan-500/10",
        title: "Automatic Syncing",
        description:
            "Your accounts sync automatically, so you always have the latest balance information without any effort.",
    },
    {
        icon: <Shield className="w-12 h-12 text-green-400" />,
        iconBg: "bg-green-500/10",
        title: "Bank-Grade Security",
        description:
            "Your data is encrypted end-to-end. We use the same security standards as major financial institutions.",
    },
    {
        icon: <Lock className="w-12 h-12 text-rose-400" />,
        iconBg: "bg-rose-500/10",
        title: "Private by Design",
        description:
            "We never sell your personal data. Your financial information stays yours, always.",
    },
];

export default function OnboardingTutorial() {
    const navigate = useNavigate();
    const { logout } = useAuth();
    const [currentSlide, setCurrentSlide] = useState(0);

    const isLastSlide = currentSlide === slides.length - 1;

    const handleNext = () => {
        if (isLastSlide) {
            navigate("/select-bank");
        } else {
            setCurrentSlide((prev) => prev + 1);
        }
    };

    const handlePrev = () => {
        if (currentSlide > 0) {
            setCurrentSlide((prev) => prev - 1);
        }
    };

    const handleSkip = () => {
        navigate("/select-bank");
    };

    const slide = slides[currentSlide];

    return (
        <div className="min-h-screen bg-slate-950 flex flex-col">
            {/* Header */}
            <header className="border-b border-slate-800 sticky top-0 z-50 bg-slate-950/95 backdrop-blur">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex items-center justify-between">
                    <div className="flex items-center gap-3">
                        <div className="w-8 h-8 rounded-lg bg-gradient-to-br from-emerald-500 via-blue-500 to-emerald-500 flex items-center justify-center">
                            <LineChart size={18} className="text-white" />
                        </div>
                        <span className="text-xl font-bold text-white">
                            NetWorth
                        </span>
                    </div>
                    <button
                        onClick={() => logout()}
                        className="flex items-center gap-2 px-4 py-2 rounded-lg border border-slate-700 text-gray-300 hover:text-white hover:border-slate-600 transition-colors"
                    >
                        <LogOut size={18} />
                        <span className="text-sm">Sign Out</span>
                    </button>
                </div>
            </header>

            <div className="flex-1 flex items-center justify-center p-4">
                <div className="max-w-lg w-full">
                    {/* Slide Content */}
                    <div className="text-center space-y-6 mb-12">
                        <div
                            className={`w-24 h-24 ${slide.iconBg} rounded-2xl mx-auto flex items-center justify-center transition-all duration-300`}
                        >
                            {slide.icon}
                        </div>
                        <div className="space-y-3">
                            <h1 className="text-3xl font-bold text-white tracking-tight">
                                {slide.title}
                            </h1>
                            <p className="text-gray-400 text-lg leading-relaxed">
                                {slide.description}
                            </p>
                        </div>
                    </div>

                    {/* Slide Indicators */}
                    <div className="flex items-center justify-center gap-2 mb-8">
                        {slides.map((_, index) => (
                            <button
                                key={index}
                                onClick={() => setCurrentSlide(index)}
                                className={`w-2 h-2 rounded-full transition-all duration-300 ${
                                    index === currentSlide
                                        ? "w-8 bg-emerald-500"
                                        : "bg-slate-700 hover:bg-slate-600"
                                }`}
                            />
                        ))}
                    </div>

                    {/* Navigation Buttons */}
                    <div className="flex items-center justify-between gap-4">
                        <button
                            onClick={handlePrev}
                            disabled={currentSlide === 0}
                            className={`flex items-center gap-2 px-6 py-3 rounded-lg border transition-all ${
                                currentSlide === 0
                                    ? "border-slate-800 text-slate-600 cursor-not-allowed"
                                    : "border-slate-700 text-gray-300 hover:text-white hover:border-slate-600"
                            }`}
                        >
                            <ArrowLeft className="w-5 h-5" />
                            Back
                        </button>

                        {!isLastSlide ? (
                            <button
                                onClick={handleSkip}
                                className="text-gray-400 hover:text-white transition-colors"
                            >
                                Skip tutorial
                            </button>
                        ) : (
                            <div className="w-[110px]" />
                        )}

                        <button
                            onClick={handleNext}
                            className="flex items-center gap-2 bg-emerald-500 hover:bg-emerald-600 text-white font-semibold py-3 px-6 rounded-lg transition-all transform hover:scale-[1.02]"
                        >
                            {isLastSlide ? "Add Your First Account" : "Next"}
                            <ArrowRight className="w-5 h-5" />
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}
