import { useAuth } from "../../contexts/AuthContext";
import { Button } from "./button";

interface SignInButtonProps {
    /** Button variant */
    variant?: "default" | "secondary" | "ghost" | "outline";
    /** Additional CSS classes */
    className?: string;
    /** Button content */
    children?: React.ReactNode;
}

export function SignInButton({
    variant = "default",
    className,
    children,
}: SignInButtonProps) {
    const { login } = useAuth();

    const handleSignIn = async () => {
        try {
            await login();
        } catch (error) {
            console.error("Sign in failed:", error);
        }
    };

    return (
        <Button variant={variant} onClick={handleSignIn} className={className}>
            {children || "Sign In with Google"}
        </Button>
    );
}
