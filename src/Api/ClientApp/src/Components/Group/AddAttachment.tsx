import FileUpload from "@components/Ui/FileUpload";
import { useParams } from "react-router-dom";

const AddAssignment = ({
  close,
  search,
}: {
  close: () => void;
  search: string;
}) => {
  const { id } = useParams();
  return (
    <>
      <FileUpload id={id as string} onSuccess={close} search={search} />
    </>
  );
};

export default AddAssignment;
