import {
  Avatar,
  Container,
  Group,
  Paper,
  Text,
  TextInput,
} from "@mantine/core";
import cx from "clsx";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import classes from "./styles/payment.module.css";

interface AccordionLabelProps {
  title: string;
  image: string;
  description: string;
}

function AccordionLabel({ title, image, description }: AccordionLabelProps) {
  return (
    <Group wrap="nowrap">
      <Avatar src={image} radius="xl" size="lg" />
      <div>
        <Text>{title}</Text>
        <Text size="sm" c="dimmed" fw={400}>
          {description}
        </Text>
      </div>
    </Group>
  );
}

const Accordion = ({ items }: any) => {
  const [isActive, setIsActive] = useState(false);
  const { t } = useTranslation();
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
        <Container style={{ marginTop: "10px" }}>
          <TextInput
            label={t("key")}
            withAsterisk
            placeholder={t("your_key") as string}
            style={{ marginBottom: "5px" }}
          />
          <TextInput
            label={t("secret")}
            withAsterisk
            type="password"
            placeholder={t("secret_key") as string}
            style={{ marginBottom: "5px" }}
          />
          <TextInput
            label={t("verify_url")}
            withAsterisk
            placeholder={t("your_verification_url") as string}
            style={{ marginBottom: "5px" }}
          />
        </Container>
      )}
    </div>
  );
};

const PaymentMethod = () => {
  const { t } = useTranslation();
  const accordionData = [
    {
      title: t("esewa"),
      image: "https://img.icons8.com/clouds/256/000000/homer-simpson.png",
      description: "Esewa says They are the best",
    },
    {
      title: t("khalti"),
      iamge: "https://img.icons8.com/clouds/256/000000/futurama-bender.png",
      description: "Khalti says THEY are the best",
    },
  ];
  return (
    <div>
      <div className="accordion">
        {accordionData.map((items, i) => (
          <Accordion key={i} items={items} />
        ))}
      </div>
    </div>
  );
};

export default PaymentMethod;
