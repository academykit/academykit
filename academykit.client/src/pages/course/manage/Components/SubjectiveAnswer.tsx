import { Paper, Spoiler } from "@mantine/core";
import removeTags from "@utils/sanitize-html";

interface IProps extends React.HTMLAttributes<HTMLDivElement> {}

const SubjectiveAnswer = ({ children }: IProps) => {
  return (
    <Paper
      mb={15}
      mt={10}
      p={"sm"}
      withBorder
      w={"70vw"}
      maw={"70vw"}
      style={{ wordWrap: "break-word" }}
    >
      <Spoiler maxHeight={30} showLabel="See more" hideLabel="Hide">
        {removeTags(children as string)}
      </Spoiler>
    </Paper>
  );
};

export default SubjectiveAnswer;
