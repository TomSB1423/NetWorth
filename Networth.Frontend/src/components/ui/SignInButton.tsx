import { useMsal } from "@azure/msal-react";
import { loginRequest } from "../../config/authConfig";
import { Button } from "./button";

interface SignInButtonProps {
    /** The identity provider to use - if not specified, uses default flow */
    provider?: "email" | "google";
    /** Button variant */
    variant?: "default" | "secondary" | "ghost" | "outline";
    /** Additional CSS classes */
    className?: string;
    /** Button content */
    children?: React.ReactNode;
}

export function SignInButton({
    provider,
    variant = "default",
    className,
    children,
}: SignInButtonProps) {
    const { instance } = useMsal();

    const handleSignIn = () => {
        const request = {
            ...loginRequest,
            ...(provider &&
                provider !== "email" && {
                    extraQueryParameters: {
                        idp: getIdpHint(provider),
                    },
                }),
        };

        instance.loginRedirect(request);
    };

    return (
        <Button variant={variant} onClick={handleSignIn} className={className}>
            {children || getDefaultLabel(provider)}
        </Button>
    );
}

function getIdpHint(provider: string): string {
    switch (provider) {
        case "google":
            return "google.com";
        default:
            return "";
    }
}

function getDefaultLabel(provider?: string): string {
    switch (provider) {
        case "google":
            return "Continue with Google";
        case "email":
            return "Continue with Email";
        default:
            return "Sign In";
    }
}
