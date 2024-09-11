import { Checkbox, Group, Text } from "@mantine/core";
import type { SignInType } from "@utils/enums";
import type { ReactNode } from "react";

interface SignInOptionProps {
  icon: ReactNode;
  title: string;
  description: string;
  signInType: SignInType;
  checked: boolean;
  onToggle: (signInType: SignInType) => void;
}

const SignInOption = ({
  icon,
  title,
  description,
  signInType,
  checked,
  onToggle,
}: SignInOptionProps) => (
  <Group grow w="50%" mb="md">
    <Group
      justify="space-between"
      style={{
        border: "1px solid #ddd",
        padding: "10px",
        borderRadius: "4px",
      }}
    >
      <Group>
        {icon}
        <Group
          style={{
            flexDirection: "column",
            gap: 0,
            alignItems: "flex-start",
          }}
        >
          <Text fw={600}>{title}</Text>
          <Text size="xs" c="dimmed">
            {description}
          </Text>
        </Group>
      </Group>
      <Checkbox checked={checked} onChange={() => onToggle(signInType)} />
    </Group>
  </Group>
);

export default SignInOption;
