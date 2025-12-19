import { cn } from "../../lib/utils";
import { ButtonHTMLAttributes, forwardRef } from "react";

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
    variant?: "default" | "secondary" | "ghost" | "outline" | "link";
    size?: "default" | "sm" | "lg" | "icon";
}

export const Button = forwardRef<HTMLButtonElement, ButtonProps>(
    ({ className, variant = "default", size = "default", ...props }, ref) => {
        const baseStyles =
            "inline-flex items-center justify-center rounded-lg font-medium transition-colors focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:ring-offset-2 focus:ring-offset-slate-950 disabled:opacity-50 disabled:pointer-events-none";

        const variants = {
            default:
                "bg-emerald-500 text-white hover:bg-emerald-600 shadow-lg shadow-emerald-500/20",
            secondary:
                "bg-slate-800 text-white hover:bg-slate-700 border border-slate-700",
            ghost: "text-gray-300 hover:text-white hover:bg-slate-800",
            outline:
                "border border-slate-700 text-gray-300 hover:bg-slate-800 hover:text-white",
            link: "text-emerald-500 hover:text-emerald-400 underline-offset-4 hover:underline",
        };

        const sizes = {
            default: "h-10 px-4 py-2 text-sm",
            sm: "h-8 px-3 text-xs",
            lg: "h-12 px-6 text-base",
            icon: "h-10 w-10",
        };

        return (
            <button
                ref={ref}
                className={cn(
                    baseStyles,
                    variants[variant],
                    sizes[size],
                    className
                )}
                {...props}
            />
        );
    }
);

Button.displayName = "Button";
