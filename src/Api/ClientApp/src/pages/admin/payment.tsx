import {
  Avatar,
  Container,
  createStyles,
  Group,
  Paper,
  Text,
  TextInput,
} from '@mantine/core';
import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';

interface AccordionLabelProps {
  title: string;
  image: string;
  description: string;
}

const useStyles = createStyles((theme) => ({
  accordionLabel: {
    display: 'flex',
    // justifyContent: "space-around",
  },
  accordionBody: {
    maxWidth: '300px',
    cursor: 'pointer',
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
  const { t } = useTranslation();
  return (
    <div style={{ marginBottom: '20px' }}>
      <Paper
        className={cx(classes.accordionBody, {
          [classes.accordionActive]: isActive,
        })}
        withBorder
        shadow={'md'}
      >
        <div
          className={classes.accordionLabel}
          onClick={() => setIsActive(!isActive)}
        >
          <AccordionLabel {...items} />
        </div>
      </Paper>
      {isActive && (
        <Container sx={{ marginTop: '10px' }}>
          <TextInput
            label={t('key')}
            withAsterisk
            placeholder={t('your_key') as string}
            sx={{ marginBottom: '5px' }}
          />
          <TextInput
            label={t('secret')}
            withAsterisk
            type="password"
            placeholder={t('secret_key') as string}
            sx={{ marginBottom: '5px' }}
          />
          <TextInput
            label={t('verify_url')}
            withAsterisk
            placeholder={t('your_verification_url') as string}
            sx={{ marginBottom: '5px' }}
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
      title: t('esewa'),
      image: 'https://img.icons8.com/clouds/256/000000/homer-simpson.png',
      description: 'Esewa says They are the best',
    },
    {
      title: t('khalti'),
      iamge: 'https://img.icons8.com/clouds/256/000000/futurama-bender.png',
      description: 'Khalti says THEY are the best',
    },
  ];
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
