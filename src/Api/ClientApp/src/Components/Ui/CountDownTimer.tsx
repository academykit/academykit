import moment from "moment";
import { FC, useEffect, useMemo, useState } from "react";
import TimeCounter from "./TimeCounter";

type Props = {
  startDateTime: string;
  time: number;
  cb: Function;
};

const CountDownTimer: FC<Props> = ({ startDateTime, cb, time }) => {
  const end = useMemo(() => moment(startDateTime).add(time, "seconds"), [time]);
  const [timer, setTimer] = useState<number>();
  const [timeOut, setTimeOut] = useState<NodeJS.Timeout>();

  useEffect(() => {
    if (timer && timer <= 0) {
      // clearTimeout(timeOut);
      clearInterval(timeOut);
      cb();
    }
  }, [timer, cb]);

  useEffect(() => {
    setTimeOut(
      setInterval(() => {
        setTimer(end.diff(moment()));
      }, 1000)
    );
  }, []);

  return timer ? (
    <TimeCounter time={timer / 1000 > 0 ? timer / 1000 : 0} />
  ) : (
    <></>
  );
};

export default CountDownTimer;
