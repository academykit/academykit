import { Button, createStyles, TextInput, UnstyledButton } from "@mantine/core";
import React from "react";
import { Check, Cross } from "@components/Icons";
import { UseFormReturnType } from "@mantine/form";

type InlineInputProps = {
  disabled?: boolean;
  onCloseEdit: Function;
  placeholder: string;
};

const useStyles = createStyles((theme) => ({
  check: {
    cursor: "pointer",
    marginLeft: "10px",
    marginTop: "auto",
  },
  cross: {
    marginTop: "auto",
    cursor: "pointer",
    marginLeft: "10px",
  },
}));

const InlineInput: React.FC<React.PropsWithChildren<InlineInputProps>> = ({
  onCloseEdit,
  placeholder,
  ...props
}) => {
  const { theme, classes } = useStyles();
  return (
    <div style={{ display: "flex" }}>
      <TextInput placeholder={placeholder} {...props} />
      <UnstyledButton type="submit" className={classes.check}>
        <Check />
      </UnstyledButton>
      <div onClick={() => onCloseEdit()} className={classes.cross}>
        <Cross />
      </div>
    </div>
  );
};

export default InlineInput;
