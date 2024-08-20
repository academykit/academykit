import { UseFormReturnType } from "@mantine/form";
import { ReactNode } from "react";

interface Props<T> {
  useFormContext: () => UseFormReturnType<T, (values: T) => T>;
  children: (form: UseFormReturnType<T, (values: T) => T>) => ReactNode;
}

export default function ContextField<T>({
  useFormContext,
  children,
}: Props<T>) {
  const form = useFormContext();
  return children(form);
}
