import { List, Paper, Title } from '@mantine/core';
import { IQuestion } from '@utils/services/questionService';
import { IUser } from '@utils/services/types';
import { useTranslation } from 'react-i18next';
interface IProps {
  title: string;
  data?: IUser[] | IQuestion[] | string[];
}

const Ranking = ({ title, data = [] }: IProps) => {
  const { t } = useTranslation();

  return (
    <>
      <Paper p={'md'} mb={10}>
        <Title order={3} mb={10}>
          {t(`${title}`)}
        </Title>
        <List type="ordered">
          {data.map((item, index) => (
            <List.Item key={index}>
              {typeof item === 'object'
                ? 'fullName' in item
                  ? (item as IUser).fullName
                  : 'name' in item
                    ? (item as IQuestion).name
                    : null
                : typeof item === 'string'
                  ? // Handle the case when it's a string (mostWrongAnsQues)
                    item
                  : null}
            </List.Item>
          ))}
        </List>
      </Paper>
    </>
  );
};

export default Ranking;
