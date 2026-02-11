import { useState, useEffect, useRef } from "react";
import {
    Wallet,
    TrendingUp,
    Beer,
    Coffee,
    ShoppingBag,
    Calculator,
    PartyPopper,
} from "lucide-react";

interface LoadingScreenProps {
    message?: string;
    userName?: string;
    isFinished?: boolean;
    onComplete?: () => void;
}

const funnySteps = [
    {
        icon: Calculator,
        text: "Crunching the numbers...",
        color: "text-blue-400",
    },
    {
        icon: Beer,
        text: `Calculating beer budget for ${new Date().getFullYear()}...`,
        color: "text-amber-500",
    },
    {
        icon: Coffee,
        text: "Analyzing caffeine dependency...",
        color: "text-amber-700",
    },
    {
        icon: ShoppingBag,
        text: "Hiding impulse purchases...",
        color: "text-rose-400",
    },
    {
        icon: TrendingUp,
        text: "Projecting your empire...",
        color: "text-emerald-400",
    },
    { icon: Wallet, text: "Finding lost wallets...", color: "text-purple-400" },
];

export function LoadingScreen({
    message,
    userName,
    isFinished,
    onComplete,
}: LoadingScreenProps) {
    const [currentStep, setCurrentStep] = useState(0);
    const [progress, setProgress] = useState(0);
    const [showWelcome, setShowWelcome] = useState(false);
    const hasCompletedRef = useRef(false);
    const onCompleteRef = useRef(onComplete);

    useEffect(() => {
        onCompleteRef.current = onComplete;
    }, [onComplete]);

    // Handle completion sequence
    useEffect(() => {
        // Reset completion guard when we are not finished
        if (!isFinished) {
            hasCompletedRef.current = false;
            return;
        }

        hasCompletedRef.current = true;
        setTimeout(() => setProgress(100), 0);

        const hasShownWelcome = sessionStorage.getItem("welcome_shown");

        // If we have a username, show welcome message
        if (userName) {
            if (!hasShownWelcome) {
                setTimeout(() => setShowWelcome(true), 0);
                sessionStorage.setItem("welcome_shown", "true");
                // Give time to read the welcome message
                const timer = setTimeout(() => {
                    onCompleteRef.current?.();
                }, 2500);
                return () => clearTimeout(timer);
            } else {
                // Already shown welcome in this session, finish immediately
                onCompleteRef.current?.();
                return;
            }
        }

        // No username, just finish quickly
        const quickTimer = setTimeout(() => {
            onCompleteRef.current?.();
        }, 500);
        return () => clearTimeout(quickTimer);
    }, [isFinished, userName]);

    useEffect(() => {
        // If we have a username but not finished yet, show welcome message after a delay
        if (userName && !isFinished) {
            const timer = setTimeout(() => setShowWelcome(true), 3000);
            return () => clearTimeout(timer);
        }
    }, [userName, isFinished]);

    useEffect(() => {
        if (isFinished) return;

        // Progress bar animation
        const progressInterval = setInterval(() => {
            setProgress((prev) => {
                if (prev >= 95) return prev;
                return prev + Math.random() * 3;
            });
        }, 200);

        // Step cycling animation
        const stepInterval = setInterval(() => {
            setCurrentStep((prev) => (prev + 1) % funnySteps.length);
        }, 2500);

        return () => {
            clearInterval(progressInterval);
            clearInterval(stepInterval);
        };
    }, [isFinished]);

    const CurrentIcon = showWelcome
        ? PartyPopper
        : funnySteps[currentStep].icon;
    const currentText = showWelcome
        ? `Welcome, ${userName}!`
        : funnySteps[currentStep].text;
    const currentColor = showWelcome
        ? "text-yellow-400"
        : funnySteps[currentStep].color;

    return (
        <div className="min-h-screen bg-slate-950 flex items-center justify-center overflow-hidden relative">
            {/* Animated background gradients */}
            <div className="absolute inset-0 overflow-hidden">
                <div className="absolute top-1/4 -left-20 w-[400px] h-[400px] bg-emerald-500/20 rounded-full blur-[120px] animate-pulse" />
                <div
                    className="absolute bottom-1/4 -right-20 w-[350px] h-[350px] bg-blue-500/20 rounded-full blur-[120px] animate-pulse"
                    style={{ animationDelay: "1s" }}
                />
                <div
                    className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[500px] h-[500px] bg-purple-500/10 rounded-full blur-[150px] animate-pulse"
                    style={{ animationDelay: "0.5s" }}
                />
            </div>

            {/* Grid pattern overlay */}
            <div className="absolute inset-0 opacity-[0.03]">
                <div
                    className="absolute inset-0"
                    style={{
                        backgroundImage: `
                            linear-gradient(rgba(16, 185, 129, 0.3) 1px, transparent 1px),
                            linear-gradient(90deg, rgba(16, 185, 129, 0.3) 1px, transparent 1px)
                        `,
                        backgroundSize: "50px 50px",
                    }}
                />
            </div>

            <div className="relative z-10 flex flex-col items-center">
                {/* Logo and main spinner */}
                <div className="relative mb-8">
                    {/* Outer spinning ring */}
                    <div className="absolute inset-0 w-32 h-32 -m-4">
                        <svg className="w-full h-full animate-spin-slow">
                            <circle
                                cx="64"
                                cy="64"
                                r="60"
                                fill="none"
                                stroke="url(#gradient1)"
                                strokeWidth="2"
                                strokeDasharray="120 280"
                                strokeLinecap="round"
                            />
                            <defs>
                                <linearGradient
                                    id="gradient1"
                                    x1="0%"
                                    y1="0%"
                                    x2="100%"
                                    y2="100%"
                                >
                                    <stop
                                        offset="0%"
                                        stopColor="rgb(16, 185, 129)"
                                    />
                                    <stop
                                        offset="50%"
                                        stopColor="rgb(59, 130, 246)"
                                    />
                                    <stop
                                        offset="100%"
                                        stopColor="rgb(168, 85, 247)"
                                    />
                                </linearGradient>
                            </defs>
                        </svg>
                    </div>

                    {/* Inner spinning ring (opposite direction) */}
                    <div className="absolute inset-0 w-32 h-32 -m-4">
                        <svg className="w-full h-full animate-spin-reverse">
                            <circle
                                cx="64"
                                cy="64"
                                r="52"
                                fill="none"
                                stroke="url(#gradient2)"
                                strokeWidth="1"
                                strokeDasharray="80 240"
                                strokeLinecap="round"
                            />
                            <defs>
                                <linearGradient
                                    id="gradient2"
                                    x1="100%"
                                    y1="0%"
                                    x2="0%"
                                    y2="100%"
                                >
                                    <stop
                                        offset="0%"
                                        stopColor="rgb(59, 130, 246)"
                                    />
                                    <stop
                                        offset="100%"
                                        stopColor="rgb(16, 185, 129)"
                                    />
                                </linearGradient>
                            </defs>
                        </svg>
                    </div>

                    {/* Center logo */}
                    <div className="w-24 h-24 rounded-2xl bg-gradient-to-br from-emerald-500 via-blue-500 to-purple-500 p-[2px] shadow-2xl shadow-emerald-500/20">
                        <div className="w-full h-full rounded-2xl bg-slate-950 flex items-center justify-center">
                            <img src="/networth-icon.svg" alt="NetWorth" className="w-12 h-12 animate-pulse" />
                        </div>
                    </div>
                </div>

                {/* Brand name */}
                <h1 className="text-3xl font-black tracking-tight text-white mb-2">
                    <span className="bg-gradient-to-r from-emerald-400 via-blue-400 to-purple-400 bg-clip-text text-transparent">
                        NetWorth
                    </span>
                </h1>

                {/* Progress bar */}
                <div className="w-64 h-1 bg-slate-800 rounded-full overflow-hidden mb-6">
                    <div
                        className="h-full bg-gradient-to-r from-emerald-500 via-blue-500 to-purple-500 rounded-full transition-all duration-300 ease-out"
                        style={{ width: `${progress}%` }}
                    />
                </div>

                {/* Dynamic step indicator */}
                <div className="flex flex-col items-center gap-3">
                    <div className="flex items-center gap-3 px-4 py-2 rounded-full bg-slate-900/80 border border-slate-800 backdrop-blur-sm min-w-[280px] justify-center">
                        <div
                            key={`icon-${currentStep}-${showWelcome}`}
                            className={`${currentColor} transition-colors duration-500 animate-fade-in`}
                        >
                            <CurrentIcon
                                size={18}
                                className="animate-bounce-gentle"
                            />
                        </div>
                        <span
                            key={`text-${currentStep}-${showWelcome}`}
                            className="text-sm font-medium text-slate-300 transition-all duration-500 animate-fade-in"
                        >
                            {currentText}
                        </span>
                    </div>

                    {message && !showWelcome && (
                        <p className="text-sm text-slate-400 animate-pulse">
                            {message}
                        </p>
                    )}
                </div>

                {/* Floating particles */}
                <div className="absolute inset-0 pointer-events-none">
                    {[...Array(6)].map((_, i) => (
                        <div
                            key={i}
                            className="absolute w-1 h-1 bg-emerald-400/40 rounded-full animate-float"
                            style={{
                                left: `${20 + i * 15}%`,
                                top: `${30 + (i % 3) * 20}%`,
                                animationDelay: `${i * 0.5}s`,
                                animationDuration: `${3 + i * 0.5}s`,
                            }}
                        />
                    ))}
                </div>
            </div>

            {/* Bottom decorative element */}
            <div className="absolute bottom-8 left-1/2 -translate-x-1/2 flex gap-2">
                {[0, 1, 2].map((i) => (
                    <div
                        key={i}
                        className="w-2 h-2 rounded-full bg-slate-700 animate-pulse"
                        style={{ animationDelay: `${i * 0.3}s` }}
                    />
                ))}
            </div>
        </div>
    );
}
