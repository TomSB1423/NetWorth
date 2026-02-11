import { forwardRef, type InputHTMLAttributes } from "react";
import { clsx } from "clsx";

interface SwitchProps extends Omit<InputHTMLAttributes<HTMLInputElement>, "type" | "onChange" | "size"> {
    checked: boolean;
    onCheckedChange: (checked: boolean) => void;
    switchSize?: "sm" | "md";
}

const Switch = forwardRef<HTMLInputElement, SwitchProps>(
    ({ checked, onCheckedChange, switchSize = "md", className, disabled, ...props }, ref) => {
        const sizes = {
            sm: {
                track: "w-9 h-5",
                thumb: "w-3.5 h-3.5",
                translate: "translate-x-4",
            },
            md: {
                track: "w-11 h-6",
                thumb: "w-4 h-4",
                translate: "translate-x-5",
            },
        };

        const sizeConfig = sizes[switchSize];

        return (
            <label
                className={clsx(
                    "relative inline-flex items-center cursor-pointer",
                    disabled && "cursor-not-allowed opacity-50",
                    className
                )}
            >
                <input
                    ref={ref}
                    type="checkbox"
                    checked={checked}
                    onChange={(e) => onCheckedChange(e.target.checked)}
                    disabled={disabled}
                    className="sr-only peer"
                    {...props}
                />
                <div
                    className={clsx(
                        sizeConfig.track,
                        "rounded-full transition-colors duration-200 ease-in-out",
                        "bg-slate-700 peer-checked:bg-emerald-500",
                        "peer-focus-visible:ring-2 peer-focus-visible:ring-emerald-500/50 peer-focus-visible:ring-offset-2 peer-focus-visible:ring-offset-slate-900"
                    )}
                >
                    <span
                        className={clsx(
                            sizeConfig.thumb,
                            "absolute top-1 left-1 rounded-full bg-white shadow-sm",
                            "transition-transform duration-200 ease-in-out",
                            checked && sizeConfig.translate
                        )}
                    />
                </div>
            </label>
        );
    }
);

Switch.displayName = "Switch";

export { Switch };
