import { cn } from "../../lib/utils";
import { useState } from "react";

type AvatarProps = React.HTMLAttributes<HTMLDivElement>;

export function Avatar({ className, ...props }: AvatarProps) {
    return (
        <div
            className={cn(
                "relative flex h-10 w-10 shrink-0 overflow-hidden rounded-full",
                className
            )}
            {...props}
        />
    );
}

type AvatarImageProps = React.ImgHTMLAttributes<HTMLImageElement>;

export function AvatarImage({
    className,
    src,
    alt,
    ...props
}: AvatarImageProps) {
    const [hasError, setHasError] = useState(false);

    if (!src || hasError) {
        return null;
    }

    return (
        <img
            src={src}
            alt={alt}
            className={cn(
                "aspect-square h-full w-full object-cover",
                className
            )}
            onError={() => setHasError(true)}
            {...props}
        />
    );
}

type AvatarFallbackProps = React.HTMLAttributes<HTMLDivElement>;

export function AvatarFallback({ className, ...props }: AvatarFallbackProps) {
    return (
        <div
            className={cn(
                "flex h-full w-full items-center justify-center rounded-full bg-gradient-to-br from-emerald-500 to-blue-500 text-white font-semibold",
                className
            )}
            {...props}
        />
    );
}
