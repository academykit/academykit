import { Route, Routes, useParams } from "react-router-dom";
import MCQDetails from ".";
import MCQPoolNav from "../Components/MCQPoolNav";
import Create from "./Create";
import EditQuestion from "./Edit";
import MCQQuestions from "./questions";
import MCQTeachers from "./teachers";
import { useOnePool } from "@utils/services/poolService";
import { useEffect } from "react";
import useNav from "@hooks/useNav";

const MCQPoolRoute = () => {
  const { id } = useParams();
  const pool = useOnePool(id as string);
  const nav = useNav();

  useEffect(() => {
    if (pool.isSuccess) {
      nav.setBreadCrumb &&
        nav.setBreadCrumb([
          { href: "/pools", title: "Pools" },
          {
            href: `/pools/${pool.data.slug}`,
            title: pool?.data?.name ?? "",
          },
        ]);
    }
  }, [pool.isSuccess, pool.isRefetching]);
  return (
    <Routes>
      <Route element={<MCQPoolNav />}>
        <Route path="/" element={<MCQDetails />} />
        <Route path="/questions" element={<MCQQuestions />} />
        <Route path="/authors" element={<MCQTeachers />} />
        <Route path="questions/create" element={<Create />} />
        <Route path="questions/edit/:slug" element={<EditQuestion />} />
      </Route>
    </Routes>
  );
};

export default MCQPoolRoute;
