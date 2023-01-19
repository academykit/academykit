import ProgressBar from "@components/Ui/ProgressBar";
import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import { Table, Title, useMantineTheme } from "@mantine/core";
import { useMyCourse } from "@utils/services/courseService";
import moment from "moment";
import { useParams } from "react-router-dom";

const UserCourse = (props: IWithSearchPagination) => {
  const { id } = useParams();
  const { data } = useMyCourse(id as string);
  const theme = useMantineTheme();

  return (
    <>
      <Title>Attended Trannings </Title>
      {data && (
        <Table>
          <thead>
            <tr>
              <th>Title</th>
              <th>Enrolled Date</th>
              <th>Progress</th>
            </tr>
          </thead>
          <tbody>
            {data.items.map((x) => (
              <tr>
                <td>{x.name}</td>
                <td>{moment(x.createdOn).format(theme.dateFormat)}</td>
                <td>
                  <ProgressBar total={100} positive={x.percentage} />
                </td>
              </tr>
            ))}
          </tbody>
        </Table>
      )}
    </>
  );
};

export default withSearchPagination(UserCourse);
