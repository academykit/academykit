import { Group } from '@mantine/core';
import { FC, useEffect, useState } from 'react';
type Props = {
  time: number;
};
const TimeCounter: FC<Props> = ({ time }) => {
  const [detailedTime, setDetailedPlan] = useState({
    day: 0,
    hour: 0,
    minute: 0,
    seconds: 0,
  });
  let timeValue = time;
  useEffect(() => {
    let hour = 0;
    let day = 0;

    let minute = 0;
    if (timeValue > 60 * 60 * 24) {
      day = Math.trunc(timeValue / (60 * 60 * 24));
      timeValue = timeValue - day * (60 * 60 * 24);
    }
    if (timeValue > 60 * 60) {
      hour = Math.trunc(timeValue / (60 * 60));
      timeValue = timeValue - hour * (60 * 60);
    }

    if (timeValue > 60) {
      minute = Math.trunc(timeValue / 60);
      timeValue = timeValue - minute * 60;
    }
    setDetailedPlan({ hour, minute, seconds: timeValue, day });
  }, [time]);

  return (
    <Group>
      {detailedTime.day > 0 && (
        <div>
          <span>{detailedTime.day.toFixed()}d:</span>
        </div>
      )}
      {detailedTime.hour > 0 && (
        <div>
          <span>{detailedTime.hour.toFixed()}h:</span>
        </div>
      )}
      {detailedTime.minute > 0 && (
        <div>
          <span>{detailedTime.minute.toFixed()}m:</span>
        </div>
      )}

      <div>
        <span>{detailedTime.seconds.toFixed()}s</span>
      </div>
    </Group>
  );
};

export default TimeCounter;
