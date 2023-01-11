import {
  Avatar,
  Container,
  createStyles,
  Group,
  Paper,
  Text,
  TextInput,
} from "@mantine/core";
import React, { useState } from "react";

interface AccordionLabelProps {
  title: string;
  image: string;
  description: string;
}

const useStyles = createStyles((theme) => ({
  accordionLabel: {
    display: "flex",
    // justifyContent: "space-around",
  },
  accordionBody: {
    maxWidth: "300px",
    cursor: "pointer",
  },
  accordionActive: {
    borderColor: theme.colors.brand[1],
  },
}));

function AccordionLabel({ title, image, description }: AccordionLabelProps) {
  return (
    <Group noWrap>
      <Avatar src={image} radius="xl" size="lg" />
      <div>
        <Text>{title}</Text>
        <Text size="sm" color="dimmed" weight={400}>
          {description}
        </Text>
      </div>
    </Group>
  );
}

const Accordion = ({ items }: any) => {
  const [isActive, setIsActive] = useState(false);
  const { theme, classes, cx } = useStyles();

  return (
    <div style={{ marginBottom: "20px" }}>
      <Paper
        className={cx(classes.accordionBody, {
          [classes.accordionActive]: isActive,
        })}
        withBorder
        shadow={"md"}
      >
        <div
          className={classes.accordionLabel}
          onClick={() => setIsActive(!isActive)}
        >
          <AccordionLabel {...items} />
        </div>
      </Paper>
      {isActive && (
        <Container sx={{ marginTop: "10px" }}>
          <TextInput
            label="Key"
            withAsterisk
            placeholder="Please enter your key"
            sx={{ marginBottom: "5px" }}
          />
          <TextInput
            label="Secret"
            withAsterisk
            type="password"
            placeholder="Your secret key"
            sx={{ marginBottom: "5px" }}
          />
          <TextInput
            label="Verify Url"
            withAsterisk
            placeholder="Your verification Url"
            sx={{ marginBottom: "5px" }}
          />
        </Container>
      )}
    </div>
  );
};

const accordionData = [
  {
    title: "Esewa",
    image: "https://img.icons8.com/clouds/256/000000/homer-simpson.png",
    description: "Esewa says They are the best",
  },
  {
    title: "Khalti",
    iamge: "https://img.icons8.com/clouds/256/000000/futurama-bender.png",
    description: "Khalti says THEY are the best",
  },
];

const PaymentMethod = () => {
  return (
    <div>
      <div className="accordion">
        {accordionData.map((items, index) => (
          <Accordion items={items} />
        ))}
      </div>
    </div>
  );
};

export default PaymentMethod;
