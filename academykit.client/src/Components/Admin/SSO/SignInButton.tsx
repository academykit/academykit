import type React from "react";

interface SignInButtonProps {
  action: string;
  Icon: React.ComponentType<{ height: number; width: number }>;
}

const SignInButton: React.FC<SignInButtonProps> = ({ action, Icon }) => {
  return (
    <form action={action} method="get">
      <button
        style={{
          border: "none",
          margin: 0,
          padding: 0,
          background: "transparent",
          cursor: "pointer",
        }}
        type="submit"
      >
        <Icon height={28} width={28} />
      </button>
    </form>
  );
};

export default SignInButton;
