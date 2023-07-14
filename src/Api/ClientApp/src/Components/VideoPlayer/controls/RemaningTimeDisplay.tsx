/* eslint-disable */
import { Text } from '@mantine/core';
import formatDuration from '@utils/formatDuration';
import React, { FC, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';

interface Props {
  played: number;
  duration: number;
}

const RemainingTimeDisplay: FC<React.PropsWithChildren<Props>> = ({
  played,
  duration,
}) => {
  const { t } = useTranslation();

  const [formattedTime, setFormattedTime] = useState('');
  useEffect(() => {
    const remainingTime = duration - duration * played;
    setFormattedTime(formatDuration(remainingTime, true, t));
  }, [duration, played]);
  return (
    <div>
      {formattedTime && (
        <div aria-live="off">
          <Text size={'xs'}>{`- ${formattedTime}`}</Text>
        </div>
      )}
    </div>
  );
};

export default RemainingTimeDisplay;
